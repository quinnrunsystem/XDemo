using Akavache.Sqlite3;

/* ==================================================================================================
 * Handling Xamarin Linker
 * Add the following class anywhere in your project to make sure Akavache.Sqlite3 will not be linked out by Xamarin
 * ================================================================================================*/
namespace XDemo.Core.Storage
{
    public static class LinkerPreserve
    {
        static LinkerPreserve()
        {
            var persistentName = typeof(SQLitePersistentBlobCache).FullName;
            var encryptedName = typeof(SQLiteEncryptedBlobCache).FullName;
        }
    }
}
