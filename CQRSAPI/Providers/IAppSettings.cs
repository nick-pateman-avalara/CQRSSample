namespace CQRSAPI.Providers
{

    interface IAppSettings
    {

        string ConnectionString { get; set; }

        void Invalidate();

    }

}
