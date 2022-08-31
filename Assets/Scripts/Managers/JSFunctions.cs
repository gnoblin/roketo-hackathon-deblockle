using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Managers
{
    public class JSCommand<TIn, TCallbackIn, TCallbackOut>
    {
        public bool IsFinished;
        public JSCallbackWrapper<TCallbackIn, TCallbackOut> Callback => callback;

        public delegate void JsCommandDelegate<in T>(T obj);
        public delegate void CallbackAwaiterDelegate(string obj);
        
        private readonly JSCallbackWrapper<TCallbackIn, TCallbackOut> callback;
        private readonly JsCommandDelegate<TIn> command;
        private bool isFinished;

        public JSCommand(JsCommandDelegate<TIn> jsCommandDelegate, JSCallbackWrapper<TCallbackIn, TCallbackOut> callback)
        {
            command = jsCommandDelegate;
            this.callback = callback;
        }

        public JSCommand<TIn, TCallbackIn, TCallbackOut> Run(TIn input)
        {
            command.Invoke(input);
            return this;
        }

        public async UniTask<JSCommand<TIn, TCallbackIn, TCallbackOut>> RunCallback(TCallbackIn input)
        {
            await callback.Run(input);
            return this;
        }

        public void AddCallbackAwaiter(CallbackAwaiterDelegate awaiter)
        {
            while (true)
            {
                
            }
        }
    }
    
    public class JSCallbackWrapper<TWrapperIn, TWrapperOut>
    {
        public bool IsFinished => isFinished;
        public TWrapperOut Output;
        public TWrapperIn Input => input;
        
        private bool isFinished;
        private Task<TWrapperOut> callback;
        private TWrapperIn input;

        public JSCallbackWrapper(Task<TWrapperOut> callback)
        {
            this.callback = callback;
        }

        public async UniTask<JSCallbackWrapper<TWrapperIn, TWrapperOut>> Run(TWrapperIn input)
        {
            this.input = input;
            callback.Start();
            await Task.WhenAll(callback);
            Output = callback.Result;
            isFinished = true;
            return this;
        }

        public UniTask GetAwaiter()
        {
            return callback.AsUniTask();
        }
    }
}