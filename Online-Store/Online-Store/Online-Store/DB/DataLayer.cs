namespace Online_Store.DB
{
    public class DataLayer
    {
        private static DataLayer _instance;
        // TODO: Add reposittories


        private DataLayer()
        {
            // TODO: Connect to database and use repositories
            

        }

        public static DataLayer Instance
        {
            get
            {
                _instance ??= new DataLayer();
                return _instance;
            }
        }
    }
}
