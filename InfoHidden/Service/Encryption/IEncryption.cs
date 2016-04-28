namespace InfoHidden.Service.Encryption
{
    public interface IEncryption
    {
        byte[] Encrypt(byte[] plaintext, uint[] key);

        byte[] Decrypt(byte[] ciphertext, uint[] key);
    }
}