using Openchain;

namespace OpenChain.Client
{
    public class TransactionInfo
    {
        public ByteString TransactionHash
        {
            get;
            set;
        }

        public ByteString MutationHash
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"M:{MutationHash} T:{TransactionHash}";
        }
    }
}