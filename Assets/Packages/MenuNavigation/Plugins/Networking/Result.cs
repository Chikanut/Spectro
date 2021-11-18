using System;
using System.Threading.Tasks;

namespace Networking {
  public struct Result<A> {
  
    //public static readonly Result<A> Bottom;
    internal readonly ResultState State;
    internal readonly A Value;
    private readonly Exception exception;

    internal Exception Exception => this.exception ?? new Exception("Default exception");//(Exception) BottomException.Default;

    public Result(A value) {
      this.State = ResultState.Success;
      this.Value = value;
      this.exception = (Exception) null;
    }

    public Result(Exception e) {
      this.State = ResultState.Faulted;
      this.exception = e;
      this.Value = default (A);
    }

    public static implicit operator Result<A>(A value) => new Result<A>(value);

    public bool IsFaulted => this.State == ResultState.Faulted;

    public bool IsSuccess => this.State == ResultState.Success;

    public override string ToString()
    {
      if (this.IsFaulted)
        return this.exception?.ToString() ?? "(Bottom)";
      A a = this.Value;
      ref A local = ref a;
      return ((object) local != null ? local.ToString() : (string) null) ?? "(null)";
    }

    public A IfFail(A defaultValue) => !this.IsFaulted ? this.Value : defaultValue;

    public A IfFail(Func<Exception, A> f) => !this.IsFaulted ? this.Value : f(this.Exception);

    public R Match<R>(Func<A, R> Succ, Func<Exception, R> Fail) => !this.IsFaulted ? Succ(this.Value) : Fail(this.Exception);

    public Result<B> Map<B>(Func<A, B> f) => !this.IsFaulted ? new Result<B>(f(this.Value)) : new Result<B>(this.Exception);

    public async Task<Result<B>> MapAsync<B>(Func<A, Task<B>> f) {
      Result<B> result;
      if (this.IsFaulted)
        result = new Result<B>(this.Exception);
      else
        result = new Result<B>(await f(this.Value));
      return result;
    }
  }
}

namespace Networking {
  public enum ResultState : byte {
    Faulted,
    Success,
  }
}