using ApiBiblio_Serveur.DataAccessLayer.Factories;

namespace ApiBiblio_Serveur.DataAccessLayer
{
    public class DAL
    {
        private LoginFactory _loginFactory = null;

        private AuteurFactory _auteurFactory = null;
        private LivreFactory _livreFactory = null;
        private CategorieFactory _CategorieFactory = null;
        private EmpruntFactory _empruntFactory = null;
        private MembreFactory _membreFactory = null;

        private RolesFactory _rolesFactory = null;
        private QuotaApiFactory _quotaApiFactory = null;

        private UtilityFactory _utilityFactory = null;

        public static string ConnectionString
        {
            get
            {
                var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
                return config.GetSection("ConnectionStrings").GetSection("Default").Value ?? string.Empty;
            }
        }

        public UtilityFactory UtilityFactory
        {
            get
            {
                if (_utilityFactory == null)
                {
                    _utilityFactory = new UtilityFactory();
                }
                return _utilityFactory;
            }
        }
        public LoginFactory LoginFactory
        {
            get
            {
                if (_loginFactory == null)
                {
                    _loginFactory = new LoginFactory();
                }
                return _loginFactory;
            }
        }


        public QuotaApiFactory QuotaApiFactory
        {
            get
            {
                if (_quotaApiFactory == null)
                {
                    _quotaApiFactory = new QuotaApiFactory();
                }
                return _quotaApiFactory;
            }
        }

        public RolesFactory RolesFactory
        {
            get
            {
                if (_rolesFactory == null)
                {
                    _rolesFactory = new RolesFactory();
                }
                return _rolesFactory;
            }
        }

        public MembreFactory MembreFactory
        {
            get
            {
                if (_membreFactory == null)
                {
                    _membreFactory = new MembreFactory();
                }
                return _membreFactory;
            }
        }
        public EmpruntFactory EmpruntFactory
        {
            get
            {
                if (_empruntFactory == null)
                {
                    _empruntFactory = new EmpruntFactory();
                }
                return _empruntFactory;
            }
        }
        public AuteurFactory AuteurFactory
        {
            get
            {
                if (_auteurFactory == null)
                {
                    _auteurFactory = new AuteurFactory();
                }
                return _auteurFactory;
            }
        }
        public LivreFactory LivreFactory
        {
            get
            {
                if (_livreFactory == null)
                {
                    _livreFactory = new LivreFactory();
                }
                return _livreFactory;
            }
        }
        public CategorieFactory CategorieFactory
        {
            get
            {
                if (_CategorieFactory == null)
                {
                    _CategorieFactory = new CategorieFactory();
                }
                return _CategorieFactory;
            }
        }
    }
}
