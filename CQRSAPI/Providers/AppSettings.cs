using Microsoft.Extensions.Configuration;

namespace CQRSAPI.Providers
{
    public class AppSettings : IAppSettings
    {

        private readonly IConfiguration _configuration;

        private string _connectionString;

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            Invalidate();
        }

        public string ConnectionString
        {
            get
            {
                if(string.IsNullOrEmpty(_connectionString) && _configuration != null)
                {
                    IConfigurationSection configurationSection = _configuration.GetSection("ConnectionStrings");
                    _connectionString =(configurationSection != null ? configurationSection.GetValue("LocalTest", string.Empty) : string.Empty);
                }

                return (_connectionString);
            }
            set => _connectionString = value;
        }

        public void Invalidate()
        {
            _connectionString = string.Empty;
        }

    }
}