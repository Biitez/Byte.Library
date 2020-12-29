namespace Byte.Library.Utilities.Combos
{
    public class Combo
    {
        public string email { get; set; }
        public string password { get; set; }
        public Status status { get; set; } = Status.Unchecked;
        public bool isValid { get; set; } = false;

        public Combo(string account, Status status)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrWhiteSpace(account) || !account.Contains(":"))
            {
                this.status = Status.InvalidFormat;
                return;
            }                
            
            try
            {
                email = account.Split(':')[0];
                password = account.Split(':')[1];

                this.status = status;
            }
            catch
            {
                this.status = Status.InvalidFormat;
            }
        }

        public Combo(string username, string password, Status status)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return;

            this.email = username;
            this.password = password;

            this.status = status;

            isValid = true;
        }
    }
}
