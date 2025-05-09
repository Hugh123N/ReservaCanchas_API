namespace Reserva.Entity.ITransactions
{
    public class ResultException<TResult> : Exception
    {
        public TResult Result { get; set; }

        public ResultException(TResult result) => Result = result;
    }
}
