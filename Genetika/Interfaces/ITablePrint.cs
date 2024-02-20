using EdsLibrary.Logging.Table;

namespace Genetika.Interfaces
{
    public interface ITablePrint<T>
    {
        TableFormatter GetTableFormatter();
        TableFormatter GetTableFormatter(T compare);
    }
}