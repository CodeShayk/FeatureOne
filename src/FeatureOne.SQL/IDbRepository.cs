namespace FeatureOne.SQL
{
    public interface IDbRepository
    {
        DbRecord[] GetByName(string name);
    }
}