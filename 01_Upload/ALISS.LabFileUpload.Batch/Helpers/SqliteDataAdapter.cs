using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Data;
using System;

namespace ALISS.LabFileUpload.Batch.Helpers
{
    public delegate void SqliteRowUpdatedEventHandler(object sender, SqliteRowUpdatedEventArgs e);
    public delegate void SqliteRowUpdatingEventHandler(object sender, SqliteRowUpdatingEventArgs e);
    public sealed class SqliteDataAdapter : DbDataAdapter
    {
        public event SqliteRowUpdatedEventHandler RowUpdated;

        public event SqliteRowUpdatingEventHandler RowUpdating;

        public SqliteDataAdapter() { }

        public SqliteDataAdapter(SqliteCommand selectCommand)
        {
            SelectCommand = selectCommand;
        }

        public SqliteDataAdapter(string selectCommandText, SqliteConnection selectConnection)
            : this(new SqliteCommand(selectCommandText, selectConnection)) { }

        public SqliteDataAdapter(string selectCommandText, string selectConnectionString)
            : this(selectCommandText, new SqliteConnection(selectConnectionString)) { }

        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command,
                                                                     StatementType statementType,
                                                                      DataTableMapping tableMapping)
        {
            return new SqliteRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
        }

        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command,
                                                                       StatementType statementType,
                                                                        DataTableMapping tableMapping)
        {
            return new SqliteRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
        }

        protected override void OnRowUpdated(RowUpdatedEventArgs value)
        {
            //base.OnRowUpdated(value);
            if (RowUpdated != null && value is SqliteRowUpdatedEventArgs)
                RowUpdated(this, (SqliteRowUpdatedEventArgs)value);
        }

        protected override void OnRowUpdating(RowUpdatingEventArgs value)
        {
            if (RowUpdating != null && value is SqliteRowUpdatingEventArgs)
                RowUpdating(this, (SqliteRowUpdatingEventArgs)value);
        }

        public new SqliteCommand DeleteCommand
        {
            get { return (SqliteCommand)base.DeleteCommand; }
            set { base.DeleteCommand = value; }
        }

        public new SqliteCommand SelectCommand
        {
            get { return (SqliteCommand)base.SelectCommand; }
            set { base.SelectCommand = value; }
        }

        public new SqliteCommand UpdateCommand
        {
            get { return (SqliteCommand)base.UpdateCommand; }
            set { base.UpdateCommand = value; }
        }

        public new SqliteCommand InsertCommand
        {
            get { return (SqliteCommand)base.InsertCommand; }
            set { base.InsertCommand = value; }
        }
    }

    public class SqliteRowUpdatingEventArgs : RowUpdatingEventArgs
    {
        public SqliteRowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType,
                                          DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) { }
    }

    public class SqliteRowUpdatedEventArgs : RowUpdatedEventArgs
    {
        public SqliteRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType,
                                         DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) { }
    }
}