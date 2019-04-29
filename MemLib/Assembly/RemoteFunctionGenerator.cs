using System;
using System.Linq;
using System.Linq.Expressions;
using MemLib.Assembly.CallingConventions;
using MemLib.Internals;

namespace MemLib.Assembly {
    public class RemoteFunctionGenerator {
        private readonly RemoteProcess m_Process;

        public RemoteFunctionGenerator(RemoteProcess process) {
            m_Process = process;
        }

        public T GetFunction<T>() where T : class {
            return (T)(object)FunctionForDelegate(typeof(T));
        }

        public T Execute<T>(IntPtr address, CallingConvention callingConvention, params dynamic[] parameters) {
            var calling = callingConvention == CallingConvention.Default ?
                CallingConventionSelector.Get(m_Process.Is64Bit ? CallingConvention.FastCall64 : CallingConvention.StdCall) :
                CallingConventionSelector.Get(m_Process.Is64Bit ? CallingConvention.FastCall64 : callingConvention);

            //var arr = new IMarshalledValue[parameters.Length];
            //for (var i = 0; i < parameters.Length; i++) {
            //    arr[i] = MarshalValue.Marshal(m_Process, parameters[i], byRef[i]);
            //}
            //var marshalledParameters = arr.Cast<IMarshalledValue>().ToArray();
            var marshalledParameters = parameters.Select(p => MarshalValue.Marshal(m_Process, p)).Cast<IMarshalledValue>().ToArray();

            AssemblyTransaction asm;
            using (asm = m_Process.Assembly.BeginTransaction()) {
                asm.Append(calling.FormatCall(address, marshalledParameters.Select(p => p.Reference).ToArray()));
            }

            //TODO FIX THIS SHIT
            //for (var i = 0; i < parameters.Length; i++) {
            //    if (!byRef[i]) continue;
            //    var val = MarshalValue.MarshalToManaged(m_Process, marshalledParameters[i].Reference, parameters[i]);
            //    Console.WriteLine($"parameters[{i}] = {val.ToString("X")}");
            //}
            foreach (var parameter in marshalledParameters) {
                parameter.Dispose();
            }
            return MarshalType<T>.PtrToObject(m_Process, asm.GetExitCode<IntPtr>());
        }
        
        internal Delegate FunctionForDelegate(Type delegateType) {
            var invoke = delegateType.GetMethod("Invoke");
            if (invoke == null) return null;
            if (!(delegateType.GetCustomAttributes(typeof(RemoteFunctionAttribute), false).FirstOrDefault() is RemoteFunctionAttribute remoteFuncAttrib)) return null;

            var returnType = invoke.ReturnType;

            var execute = GetType().GetMethod("Execute")?.MakeGenericMethod(returnType);
            if (execute == null) return null;
            
            IntPtr functionAddress;
            var callingConvention = remoteFuncAttrib.CallingConvention;
            if (!string.IsNullOrEmpty(remoteFuncAttrib.DllName)) {
                var mod = m_Process.Modules[remoteFuncAttrib.DllName];
                if (mod == null) mod = m_Process.Modules.Inject(remoteFuncAttrib.DllName);
                if (mod == null) return null;
                var func = mod[remoteFuncAttrib.EntryPoint];
                if (func == null) return null;
                functionAddress = func.BaseAddress;
            } else {
                if (remoteFuncAttrib.RemoteAddress == IntPtr.Zero) return null;
                functionAddress = remoteFuncAttrib.RemoteAddress;
            }
            
            var parameterExpressions = invoke.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
            var isByRef = parameterExpressions.Select(p => p.IsByRef).ToArray();
            var hasByRef = isByRef.FirstOrDefault(r => r);

            Expression body;
            if (hasByRef) {
                body = Expression.Call(
                    Expression.Constant(this),
                    execute,
                    Expression.Constant(functionAddress),
                    Expression.Constant(callingConvention),
                    Expression.NewArrayInit(typeof(object),
                        parameterExpressions.Select(p => Expression.Convert(p, typeof(object))))
                );
            } else {
                body = Expression.Call(
                    Expression.Constant(this),
                    execute,
                    Expression.Constant(functionAddress),
                    Expression.Constant(callingConvention),
                    Expression.NewArrayInit(typeof(object),
                        parameterExpressions.Select(p => Expression.Convert(p, typeof(object))))
                );
            }

            var lambda = Expression.Lambda(delegateType, body, delegateType.Name, parameterExpressions);
            return lambda.Compile();
        }
    }
}