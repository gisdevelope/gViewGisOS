using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace gView.Framework.Db
{
    public enum DBType { odbc, oledb, sql, oracle, npgsql, unkonwn }
    public enum dataType { integer, real, date, text, boolean, currency, unknown }

    /// <summary>
    /// Zusammenfassung f�r DBConnection.
    /// </summary>
    public class CommonDbConnection : ICommonDbConnection
    {
        protected string _connectionString = "", m_errMsg = "";
        protected DBType _dbtype = DBType.oledb;
        protected DataTable _schemaTable = null;
        protected DbDataAdapter _updateAdapter = null;
        protected SqlConnection _sqlConnection = null;
        protected OleDbConnection _oleDbConnection = null;
        private Exception _lastException = null;

        public CommonDbConnection()
        {
        }
        public CommonDbConnection(string conn)
        {
            _connectionString = conn;
        }

        public void Dispose()
        {
            if (_updateAdapter != null)
            {
                _updateAdapter.Dispose();
                _updateAdapter = null;
            }
            if (_schemaTable != null)
            {
                _schemaTable.Dispose();
                _schemaTable = null;
            }
            GC.Collect(0);
        }

        public void CloseConnection()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed) _sqlConnection.Close();
                _sqlConnection.Dispose();
                _sqlConnection = null;
            }
            if (_oleDbConnection != null)
            {
                if (_oleDbConnection.State != ConnectionState.Closed) _oleDbConnection.Close();
                _oleDbConnection.Dispose();
                _oleDbConnection = null;
            }
        }

        public System.Data.Common.DbConnection OpenConnection()
        {
            try
            {
                CloseConnection();
                switch (_dbtype)
                {
                    case DBType.oledb:
                        _oleDbConnection = new OleDbConnection(_connectionString);
                        _oleDbConnection.Open();
                        return _oleDbConnection;
                    case DBType.sql:
                        _sqlConnection = new SqlConnection(_connectionString);
                        _sqlConnection.Open();
                        return _sqlConnection;
                }
                return null;
            }
            catch (Exception ex)
            {
                try { CloseConnection(); }
                catch { }
                m_errMsg = ex.Message;
                _lastException = ex;
                return null;
            }
        }

        public DbConnectionString DbConnectionString
        {
            set
            {
                if (value != null)
                    this.ConnectionString2 = value.ConnectionString;
            }
        }
        public string ConnectionString2
        {
            set
            {
                if (value.ToLower().IndexOf("sql:") == 0 ||
                   value.ToLower().IndexOf("sqlclient:") == 0)
                {
                    _connectionString = value.Substring(value.IndexOf(":") + 1, value.Length - value.IndexOf(":") - 1);
                    _dbtype = DBType.sql;
                }
                else if (value.ToLower().IndexOf("odbc:") == 0)
                {
                    _connectionString = value.Substring(value.IndexOf(":") + 1, value.Length - value.IndexOf(":") - 1);
                    _dbtype = DBType.odbc;
                }
                else if (value.ToLower().IndexOf("oracle:") == 0 ||
                        value.ToLower().IndexOf("oracleclient:") == 0)
                {
                    _connectionString = value.Substring(value.IndexOf(":") + 1, value.Length - value.IndexOf(":") - 1);
                    _dbtype = DBType.oracle;
                }
                else if (value.ToLower().IndexOf("npgsql:") == 0)
                {
                    _connectionString = value.Substring(value.IndexOf(":") + 1, value.Length - value.IndexOf(":") - 1);
                    _dbtype = DBType.npgsql;
                }
                else
                {
                    if (gView.Framework.system.Wow.Is64BitProcess)
                    {
                        _connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=" + value;
                    }
                    else
                    {
                        _connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + value;
                    }

                    _dbtype = DBType.oledb;
                }
            }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        public DBType dbType
        {
            get { return _dbtype; }
            set { _dbtype = value; }
        }
        public string dbTypeString
        {
            get
            {
                return _dbtype.ToString();
            }
            set
            {
                switch (value.ToLower())
                {
                    case "oledb": dbType = DBType.oledb; break;
                    case "odbc": dbType = DBType.odbc; break;
                    case "sql":
                    case "sqlserver": dbType = DBType.sql; break;
                    case "oracle": dbType = DBType.oracle; break;
                }
            }
        }
        public string errorMessage { get { return m_errMsg; } }
        public Exception lastException { get { return _lastException; } }

        public bool SQLQuery(ref DataSet ds, string sql, string table, bool writeable)
        {
            if (!writeable) return SQLQuery(ref ds, sql, table);

            if (_updateAdapter != null)
            {
                _updateAdapter.Dispose(); 
                _updateAdapter = null;
            }
            try
            {
                switch (_dbtype)
                {
                    case DBType.odbc:
                        if (_updateAdapter == null)
                        {
                            _updateAdapter = new OdbcDataAdapter(sql, _connectionString);
                            OdbcCommandBuilder commBuilder1 = new OdbcCommandBuilder((OdbcDataAdapter)_updateAdapter);
                        }
                        break;
                    case DBType.oledb:
                        if (_updateAdapter == null)
                        {
                            _updateAdapter = new OleDbDataAdapter(sql, _connectionString);
                            OleDbCommandBuilder commBuilder2 = new OleDbCommandBuilder((OleDbDataAdapter)_updateAdapter);
                        }
                        break;
                    case DBType.sql:
                        if (_updateAdapter == null)
                        {
                            _updateAdapter = new SqlDataAdapter(sql, _connectionString);
                            SqlCommandBuilder commBuilder3 = new SqlCommandBuilder((SqlDataAdapter)_updateAdapter);
                            commBuilder3.QuotePrefix = "[";
                            commBuilder3.QuoteSuffix = "]";
                        }
                        break;
                    case DBType.oracle:
                        if (_updateAdapter == null)
                        {
                            _updateAdapter = new OracleDataAdapter(sql, _connectionString);
                            OracleCommandBuilder commBuilder4 = new OracleCommandBuilder((OracleDataAdapter)_updateAdapter);
                        }
                        break;
                    case DBType.npgsql:
                        if (_updateAdapter == null)
                        {
                            DbProviderFactory dbfactory = DataProvider.PostgresProvider;
                            _updateAdapter = dbfactory.CreateDataAdapter();

                            // Command Builder brauchts auch... ?!
                            DbCommandBuilder cb = dbfactory.CreateCommandBuilder();
                            cb.DataAdapter = _updateAdapter;

                            DbCommand command = dbfactory.CreateCommand();

                            _updateAdapter.SelectCommand = command;
                            _updateAdapter.SelectCommand.CommandText = sql;
                            _updateAdapter.SelectCommand.Connection = dbfactory.CreateConnection();
                            _updateAdapter.SelectCommand.Connection.ConnectionString = _connectionString;
                             
                            break;
                        }
                        break;
                }
                _updateAdapter.Fill(ds, table);
            }
            catch (Exception e)
            {
                if (_updateAdapter != null)
                {
                    _updateAdapter.Dispose();
                    _updateAdapter = null;
                }
                m_errMsg = "QUERY WRITEABLE (" + sql + "): " + e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        public bool Update(DataTable table)
        {
            try
            {
                _updateAdapter.Update(table);

                _updateAdapter.Dispose();
                _updateAdapter = null;
            }
            catch (Exception e)
            {
                if (_updateAdapter != null)
                {
                    _updateAdapter.Dispose();
                    _updateAdapter = null;
                }
                m_errMsg = "UPDATE TABLE " + table + ": " + e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        public bool UpdateData(ref DataSet ds, string table)
        {
            return Update(ds.Tables[table]);
        }
        public void ReleaseUpdateAdapter()
        {
            if (_updateAdapter != null)
            {
                _updateAdapter.Dispose();
                _updateAdapter = null;
            }
        }

        public DbDataAdapter UpdateAdapter { get { return _updateAdapter; } }

        public DataTable Select(string fields, string from)
        {
            return Select(fields, from, "", "", false);
        }
        public DataTable Select(string fields, string from, string where)
        {
            return Select(fields, from, where, "", false);
        }
        public DataTable Select(string fields, string from, string where, string orderBy)
        {
            return Select(fields, from, where, orderBy, false);
        }
        public DataTable Select(string fields, string from, string where, string orderBy, bool writeable)
        {
            DataSet ds = new DataSet();
            string sql = "SELECT " + ((fields == "") ? "*" : fields)
                               + " FROM " + from
                               + ((where == "") ? "" : " WHERE " + where)
                               + ((orderBy == "") ? "" : " ORDER BY " + orderBy);
            if (!SQLQuery(ref ds, sql, "TAB1", writeable))
            {
                return null;
            }
            return ds.Tables[0];
        }
        public bool SQLQuery(ref DataSet ds, string sql, string table)
        {
            DbDataAdapter adapter = null;
            try
            {
                switch (_dbtype)
                {
                    case DBType.odbc:
                        adapter = new OdbcDataAdapter(sql, _connectionString);
                        break;
                    case DBType.oledb:
                        adapter = new OleDbDataAdapter(sql, _connectionString);
                        break;
                    case DBType.sql:
                        adapter = new SqlDataAdapter(sql, _connectionString);
                        break;
                    case DBType.oracle:
                        adapter = new OracleDataAdapter(sql, _connectionString);
                        break;
                    case DBType.npgsql:
                        DbProviderFactory dbfactory = DataProvider.PostgresProvider;
                        adapter = dbfactory.CreateDataAdapter();
                        adapter.SelectCommand = dbfactory.CreateCommand();
                        adapter.SelectCommand.CommandText = sql;
                        adapter.SelectCommand.Connection = dbfactory.CreateConnection();
                        adapter.SelectCommand.Connection.ConnectionString = _connectionString;
                        break;

                }
                adapter.Fill(ds, table);
                adapter.Dispose();
                adapter = null;
            }
            catch (Exception e)
            {
                if (adapter != null) adapter.Dispose();
                m_errMsg = "QUERY (" + sql + "): " + e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        public bool SQLQuery(ref DataSet ds, string sql, string table, DataRow refRow)
        {
            string field = getFieldPlacehoder(sql);
            while (field != "")
            {
                if (refRow[field] == null)
                {
                    m_errMsg = "Field " + field + " not found in DataRow !!";
                    return false;
                }
                sql = sql.Replace("[" + field + "]", refRow[field].ToString());
                field = getFieldPlacehoder(sql);
            }
            return SQLQuery(ref ds, sql, table);
        }
        public bool SQLQuery(string sql, ref XmlNode feature)
        {
            return SQLQuery(sql, ref feature, true);
        }
        public bool SQLQuery(string sql, ref XmlNode feature, bool one2n)
        {
            string field = getFieldPlacehoder(sql);
            while (field != "")
            {
                string val = getFieldValue(feature, field);
                if (val == "") return false;
                sql = sql.Replace("[" + field + "]", val);
                field = getFieldPlacehoder(sql);
            }

            DataSet ds = new DataSet();
            if (!SQLQuery(ref ds, sql, "JOIN")) return false;
            if (ds.Tables["JOIN"].Rows.Count == 0) return false;
            DataRow row = ds.Tables["JOIN"].Rows[0];

            XmlNodeList fields = feature.SelectNodes("FIELDS");
            if (fields.Count == 0) return false;

            int rowCount = ds.Tables["JOIN"].Rows.Count;
            XmlAttribute attr;
            foreach (DataColumn col in ds.Tables["JOIN"].Columns)
            {
                XmlNode fieldNode = feature.OwnerDocument.CreateNode(XmlNodeType.Element, "FIELD", "");

                attr = feature.OwnerDocument.CreateAttribute("name");
                attr.Value = col.ColumnName;
                fieldNode.Attributes.Append(attr);
                attr = feature.OwnerDocument.CreateAttribute("value");
                if (!one2n)
                {
                    attr.Value = row[col.ColumnName].ToString();
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table width='100%' cellpadding='0' cellspacing='0' border='0'>");

                    int count = 1;
                    foreach (DataRow row2 in ds.Tables["JOIN"].Rows)
                    {
                        if (count < rowCount)
                            sb.Append("<tr><td nowrap style='border-bottom: gray 1px solid'>");
                        else
                            sb.Append("<tr><td nowrap>");
                        string val = row2[col.ColumnName].ToString().Trim();
                        if (val == "") val = "&nbsp;";
                        sb.Append(val + "</td></tr>");
                        count++;
                    }
                    sb.Append("</table>");
                    attr.Value = sb.ToString();
                }
                fieldNode.Attributes.Append(attr);
                attr = feature.OwnerDocument.CreateAttribute("type");
                attr.Value = "12";
                fieldNode.Attributes.Append(attr);
                fields[0].AppendChild(fieldNode);
            }
            return true;
        }

        public DbDataReader DataReader(string sql, out DbConnection connection)
        {
            DbDataReader reader = null;
            connection = null;
            try
            {
                switch (_dbtype)
                {
                    case DBType.odbc:
                        connection = new OdbcConnection(_connectionString);
                        OdbcCommand odbcCommand = new OdbcCommand(sql, (OdbcConnection)connection);
                        connection.Open();
                        return odbcCommand.ExecuteReader();
                    case DBType.oledb:
                        connection = new OleDbConnection(_connectionString);
                        OleDbCommand oledbCommand = new OleDbCommand(sql, (OleDbConnection)connection);
                        connection.Open();
                        return oledbCommand.ExecuteReader();
                    case DBType.sql:
                        connection = new SqlConnection(_connectionString);
                        SqlCommand sqlCommand = new SqlCommand(sql, (SqlConnection)connection);
                        connection.Open();
                        return sqlCommand.ExecuteReader();
                    case DBType.oracle:
                        connection = new OracleConnection(_connectionString);
                        OracleCommand oracleCommand = new OracleCommand(sql, (OracleConnection)connection);
                        connection.Open();
                        return oracleCommand.ExecuteReader();
                    case DBType.npgsql:
                        DbProviderFactory dbfactory = DataProvider.PostgresProvider;
                        connection = dbfactory.CreateConnection();
                        connection.ConnectionString = _connectionString;
                        DbCommand dbcommand = dbfactory.CreateCommand();
                        dbcommand.CommandText = sql;
                        dbcommand.Connection = connection;
                        connection.Open();
                        return dbcommand.ExecuteReader();
                }

                return null;
            }
            catch (Exception e)
            {
                if (connection != null) connection.Dispose();
                if (reader != null) reader.Dispose();

                connection = null;
                m_errMsg = "QUERY (" + sql + "): " + e.Message;
                _lastException = e;
                return null;
            }
        }

        public object QuerySingleField(string sql, string FieldName)
        {
            try
            {
                DataSet ds = new DataSet();
                if (SQLQuery(ref ds, sql, "FIELD"))
                {
                    if (ds.Tables["FIELD"].Rows.Count > 0)
                    {
                        return ds.Tables["FIELD"].Rows[0][FieldName];
                    }
                }
            }
            catch (Exception ex)
            {
                m_errMsg = ex.Message;
                _lastException = ex;
            }
            return null;
        }
        protected string getFieldPlacehoder(string str)
        {
            int pos = str.IndexOf("[");
            if (pos == -1) return "";
            int pos2 = str.IndexOf("]");
            if (pos2 == -1) return "";

            return str.Substring(pos + 1, pos2 - pos - 1);
        }

        protected string getFieldValue(XmlNode feature, string name)
        {
            XmlNodeList fields = feature.SelectNodes("FIELDS/FIELD");
            name = name.ToUpper();
            foreach (XmlNode field in fields)
            {
                string fieldname = field.Attributes["name"].Value.ToString().ToUpper();

                if (fieldname == name || shortName(fieldname).ToUpper() == name)
                {
                    string val = field.Attributes["value"].Value.ToString();
                    return val;
                }
            }
            return "";
        }

        protected string shortName(string name)
        {
            name.Trim();
            int index = name.IndexOf(".");
            while (index != -1)
            {
                name = name.Substring(index + 1, name.Length - index - 1);
                index = name.IndexOf(".");
            }
            return name;
        }

        public bool insertInto(string tab, string[] fields, string[] values)
        {
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {
                connection = new OleDbConnection(_connectionString);
                connection.Open();

                string sql = "INSERT INTO " + tab + " ";
                if (fields != null)
                {
                    sql += "(";
                    for (int i = 0; i < fields.Length; i++)
                    {
                        sql += fields[i];
                        if (i < (fields.Length - 1)) sql += ",";
                    }
                    sql += ") ";
                }
                sql += "values (";
                for (int i = 0; i < values.Length; i++)
                {
                    sql += values[i];
                    if (i < (values.Length - 1)) sql += ",";
                }
                sql += ");";

                command = new OleDbCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        public bool createIndex(string name, string table, string field, bool unique)
        {
            return createIndex(name, table, field, unique, false);
        }
        public bool createIndex(string name, string table, string field, bool unique, bool grouped)
        {
            switch (_dbtype)
            {
                case DBType.oledb:
                    return createIndexOleDb(name, table, field, unique);
                case DBType.sql:
                    return createIndexSql(name, table, field, unique, grouped);
                case DBType.npgsql:
                    return createIndexNpgsql(name, table, field, unique, grouped);
            }
            return false;
        }

        private bool createIndexOleDb(string name, string table, string field, bool unique)
        {
            if (_dbtype != DBType.oledb) return false;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {
                connection = new OleDbConnection(_connectionString);
                connection.Open();
                string sql = "CREATE " + (unique ? "UNIQUE " : "") + "INDEX " + name + " ON " + table + " (" + field + ");";

                command = new OleDbCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();

            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        private bool createIndexSql(string name, string table, string field, bool unique, bool grouped)
        {
            if (_dbtype != DBType.sql) return false;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();
                string sql = "CREATE " + (unique ? "UNIQUE " : "") + (grouped ? "CLUSTERED " : "NONCLUSTERED ") + "INDEX " + name + " ON " + table + " (" + field + ") WITH PAD_INDEX,FILLFACTOR=100;";

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();

            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        private bool createIndexNpgsql(string name, string table, string field, bool unique, bool grouped)
        {
            // CREATE INDEX fc_waterways_nid ON fc_waterways (fdb_nid);
            // CREATE UNIQUE INDEX fc_waterways_id ON fc_waterways (fdb_oid);
            // ALTER TABLE fc_waterways CLUSTER ON fc_waterways_id;
            if (_dbtype != DBType.npgsql) return false;
            try
            {
                DbProviderFactory factory = DataProvider.PostgresProvider;
                using (DbConnection connection = factory.CreateConnection())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    string sql1 = "CREATE " + (unique ? "UNIQUE" : "") + " INDEX \"" + name + "\" ON \"" + table + "\" (\"" + field + "\")";
                    string sql2 = grouped ? "ALTER TABLE \"" + table + "\" CLUSTER ON \"" + name + "\"" : String.Empty;

                    using (DbCommand command = factory.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = sql1;
                        command.ExecuteNonQuery();
                    }
                    if (!String.IsNullOrEmpty(sql2))
                    {
                        using (DbCommand command = factory.CreateCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = sql2;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        public bool createTable(string name, string[] fields, string[] dataTypes)
        {
            switch (_dbtype)
            {
                case DBType.oledb:
                    return createTableOleDB(name, fields, dataTypes);
                case DBType.sql:
                    return createTableSql(name, fields, dataTypes);
                case DBType.npgsql:
                    return createTableNpgsql(name, fields, dataTypes);
            }
            m_errMsg = "Not Implemented...";
            return false;
        }

        public bool ExecuteNoneQuery(string sql)
        {
            switch (_dbtype)
            {
                case DBType.oledb:
                    return ExecuteNoneQueryOleDb(sql);
                case DBType.sql:
                    return ExecuteNoneQuerySql(sql);
                case DBType.npgsql:
                    return ExecuteNoneQueryNpqsql(sql);
            }
            m_errMsg = "Not Implemented...";
            return false;
        }
        private bool ExecuteNoneQueryOleDb(string sql)
        {
            if (_dbtype != DBType.oledb) return false;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {
                if (_oleDbConnection == null)
                {
                    connection = new OleDbConnection(_connectionString);
                    connection.Open();
                }
                else
                {
                    connection = _oleDbConnection;
                    if (connection.State != ConnectionState.Open) connection.Open();
                }
                command = new OleDbCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();

                if (_oleDbConnection == null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        private bool ExecuteNoneQuerySql(string sql)
        {
            if (_dbtype != DBType.sql) return false;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                if (_sqlConnection == null)
                {
                    connection = new SqlConnection(_connectionString);
                    connection.Open();
                }
                else
                {
                    connection = _sqlConnection;
                    if (connection.State != ConnectionState.Open) connection.Open();
                }

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();

                if (_sqlConnection == null)
                {
                    connection.Close();
                    connection.Dispose();
                }

            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        private bool ExecuteNoneQueryNpqsql(string sql)
        {
            if (_dbtype != DBType.npgsql) return false;
            try
            {
                DbProviderFactory factory = DataProvider.PostgresProvider;
                using (DbConnection connection = factory.CreateConnection())
                using (DbCommand command = factory.CreateCommand())
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        public bool RenameTable(string name, string newName)
        {
            switch (_dbtype)
            {
                case DBType.sql:
                    return ExecuteNoneQuerySql("sp_rename '" + name + "','" + newName + "'");
                case DBType.npgsql:
                    if (!name.Contains("\"")) name = "\"" + name + "\"";
                    if (!newName.Contains("\"")) newName = "\"" + newName + "\"";
                    return ExecuteNoneQueryNpqsql("ALTER TABLE "+name+" RENAME TO "+newName);
            }
            m_errMsg = "Not Implemented...";
            return false;
        }
        public bool dropIndex(string name)
        {
            switch (_dbtype)
            {
                case DBType.oledb:
                    return dropIndexOleDb(name);
                case DBType.sql:
                    return dropIndexSql(name);
            }
            m_errMsg = "Not Implemented...";
            return false;
        }
        private bool dropIndexOleDb(string name)
        {
            if (_dbtype != DBType.oledb) return false;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {
                connection = new OleDbConnection(_connectionString);
                connection.Open();
                string sql = "DROP INDEX " + name + ";";

                command = new OleDbCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();

            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        private bool dropIndexSql(string name)
        {
            if (_dbtype != DBType.sql) return false;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();
                string sql = "DROP INDEX " + name + ";";

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();

            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        private bool createTableOleDB(string name, string[] fields, string[] dataTypes)
        {
            if (_dbtype != DBType.oledb) return false;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {
                connection = new OleDbConnection(_connectionString);
                connection.Open();

                string sql = "CREATE TABLE " + name + " (";
                for (int i = 0; i < fields.Length; i++)
                {
                    string field = fields[i];
                    if (field == "KEY") field = "KEY_";
                    if (field == "USER") field = "USER_";
                    if (field == "TEXT") field = "TEXT_";
                    if (field == "COUNT") field = "COUNT_";

                    sql += field + " " + dataTypes[i];
                    if (i < (fields.Length - 1)) sql += ",";
                }
                sql += ");";

                command = new OleDbCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        private bool createTableSql(string name, string[] fields, string[] dataTypes)
        {
            if (_dbtype != DBType.sql) return false;
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();

                string sql = "CREATE TABLE [" + name + "] (";
                for (int i = 0; i < fields.Length; i++)
                {
                    string field = fields[i];
                    if (field == "KEY") field = "KEY_";
                    if (field == "USER") field = "USER_";
                    if (field == "TEXT") field = "TEXT_";

                    sql += "[" + field + "] " + dataTypes[i];
                    if (i < (fields.Length - 1)) sql += ",";
                }
                sql += ") ON [PRIMARY]";
                if (sql.IndexOf("[image]") != -1) sql += " TEXTIMAGE_ON [PRIMARY]";
                sql += ";";

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        private bool createTableNpgsql(string name, string[] fields, string[] dataTypes)
        {
            if (_dbtype != DBType.npgsql) return false;
            try
            {
                DbProviderFactory factory = DataProvider.PostgresProvider;
                using (DbConnection connection = factory.CreateConnection())
                using (DbCommand command = factory.CreateCommand())
                {
                    string sql = "create table \"" + name + "\" (";
                    for (int i = 0; i < fields.Length; i++)
                    {
                        string field = fields[i];

                        sql += "\"" + field + "\" " + dataTypes[i];
                        if (i < (fields.Length - 1)) sql += ",";
                    }
                    sql += ") WITHOUT OIDS;";

                    connection.ConnectionString = _connectionString;
                    command.CommandText = sql;
                    command.Connection = connection;
                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        public bool dropTable(string name)
        {
            switch (_dbtype)
            {
                case DBType.oledb:
                    return dropTableOleDb(name);
                case DBType.sql:
                    return dropTableSql(name);
                case DBType.npgsql:
                    return dropTableNpgsql(name);
            }
            m_errMsg = "Not Implemented...";
            return false;
        }
        private bool dropTableOleDb(string name)
        {
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {
                connection = new OleDbConnection(_connectionString);
                connection.Open();

                string sql = "DROP TABLE " + name + ";";

                command = new OleDbCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        private bool dropTableSql(string name)
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();

                string sql = "DROP TABLE " + name + ";";

                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception e)
            {
                if (command != null) command.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        private bool dropTableNpgsql(string name)
        {
            try
            {
                DbProviderFactory factory = DataProvider.PostgresProvider;
                using (DbConnection connection = factory.CreateConnection())
                using (DbCommand command = factory.CreateCommand())
                {
                    connection.ConnectionString = _connectionString;
                    command.CommandText = "drop table " + name;
                    command.Connection = connection;
                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }

        public bool GetSchema(string name)
        {
            try
            {
                switch (_dbtype)
                {
                    case DBType.oledb:
                        OleDbConnection conn = null;
                        OleDbCommand comm = null;
                        OleDbDataReader reader = null;

                        using (conn = new OleDbConnection(_connectionString))
                        {
                            conn.Open();
                            if (conn.State == ConnectionState.Open)
                            {
                                comm = new OleDbCommand("SELECT * FROM " + name, conn);
                                reader = comm.ExecuteReader(CommandBehavior.SchemaOnly);
                                _schemaTable = reader.GetSchemaTable();
                            }
                        }
                        break;
                    case DBType.sql:
                        SqlConnection sqlconn = null;
                        SqlCommand sqlcomm = null;
                        SqlDataReader sqlreader = null;

                        using (sqlconn = new SqlConnection(_connectionString))
                        {
                            sqlconn.Open();
                            if (sqlconn.State == ConnectionState.Open)
                            {
                                sqlcomm = new SqlCommand("SELECT * FROM " + name, sqlconn);
                                sqlreader = sqlcomm.ExecuteReader(CommandBehavior.SchemaOnly);
                                _schemaTable = sqlreader.GetSchemaTable();
                            }
                        }
                        break;
                    case DBType.oracle:
                        OracleConnection oconn = null;
                        OracleCommand ocomm = null;
                        OracleDataReader oreader = null;

                        using (oconn = new OracleConnection(_connectionString))
                        {
                            oconn.Open();
                            if (oconn.State == ConnectionState.Open)
                            {
                                ocomm = new OracleCommand("SELECT * FROM " + name, oconn);
                                oreader = ocomm.ExecuteReader(CommandBehavior.SchemaOnly);
                                _schemaTable = oreader.GetSchemaTable();
                            }
                        }
                        break;
                    case DBType.npgsql:
                        DbProviderFactory dbfactory = DataProvider.PostgresProvider;
                        using (DbConnection dbconn = dbfactory.CreateConnection())
                        {
                            dbconn.ConnectionString = _connectionString;
                            dbconn.Open();
                            if (dbconn.State == ConnectionState.Open)
                            {
                                DbCommand dbcomm = dbfactory.CreateCommand();
                                dbcomm.Connection = dbconn;
                                if (!name.Contains("\"")) name = "\"" + name + "\"";
                                dbcomm.CommandText = "select * from " + name+" limit 0";
                                DbDataReader dbreader = dbcomm.ExecuteReader(CommandBehavior.SchemaOnly);
                                _schemaTable = dbreader.GetSchemaTable();
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                _schemaTable = null;
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            return true;
        }
        public DataTable GetSchema2(string name)
        {
            DataTable schema = null;

            try
            {
                switch (_dbtype)
                {
                    case DBType.oledb:
                        OleDbConnection conn = null;
                        OleDbCommand comm = null;
                        OleDbDataReader reader = null;

                        using (conn = new OleDbConnection(_connectionString))
                        {
                            conn.Open();
                            if (conn.State == ConnectionState.Open)
                            {
                                comm = new OleDbCommand("SELECT * FROM " + name, conn);
                                reader = comm.ExecuteReader(CommandBehavior.SchemaOnly);
                                schema = reader.GetSchemaTable();
                            }
                        }
                        break;
                    case DBType.sql:
                        SqlConnection sqlconn = null;
                        SqlCommand sqlcomm = null;
                        SqlDataReader sqlreader = null;

                        using (sqlconn = new SqlConnection(_connectionString))
                        {
                            sqlconn.Open();
                            if (sqlconn.State == ConnectionState.Open)
                            {
                                sqlcomm = new SqlCommand("SELECT * FROM " + name, sqlconn);
                                sqlreader = sqlcomm.ExecuteReader(CommandBehavior.SchemaOnly);
                                schema = sqlreader.GetSchemaTable();
                            }
                        }
                        break;
                    case DBType.oracle:
                        OracleConnection oconn = null;
                        OracleCommand ocomm = null;
                        OracleDataReader oreader = null;

                        using (oconn = new OracleConnection(_connectionString))
                        {
                            oconn.Open();
                            if (oconn.State == ConnectionState.Open)
                            {
                                ocomm = new OracleCommand("SELECT * FROM " + name, oconn);
                                oreader = ocomm.ExecuteReader(CommandBehavior.SchemaOnly);
                                schema = oreader.GetSchemaTable();
                            }
                        }
                        break;
                    case DBType.npgsql:
                        DbProviderFactory dbfactory = DataProvider.PostgresProvider;
                        using (DbConnection dbconn = dbfactory.CreateConnection())
                        {
                            dbconn.ConnectionString = _connectionString;
                            dbconn.Open();
                            if (dbconn.State == ConnectionState.Open)
                            {
                                DbCommand dbcomm = dbfactory.CreateCommand();
                                dbcomm.Connection = dbconn;
                                if (!name.Contains("\"")) name = "\"" + name + "\"";
                                dbcomm.CommandText = "select * from " + name + " limit 0";
                                DbDataReader dbreader = dbcomm.ExecuteReader(CommandBehavior.SchemaOnly);
                                schema = dbreader.GetSchemaTable();
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                m_errMsg = e.Message;
                _lastException = e;
                return null;
            }
            return schema;
        }

        public bool createTable(DataTable tab, bool data)
        {
            try
            {
                if (this.GetSchema(tab.TableName))
                {
                    dropTable(tab.TableName);
                }
                string fields = "", dataTypes = "";
                foreach (DataColumn col in tab.Columns)
                {
                    if (fields != "")
                    {
                        fields += ";";
                        dataTypes += ";";
                    }
                    fields += col.ColumnName;
                    if (col.DataType == typeof(int))
                    {
                        dataTypes += "INTEGER";
                    }
                    else
                        dataTypes += "TEXT(50) WITH COMPRESSION";
                }

                if (createTable(tab.TableName, fields.Split(';'), dataTypes.Split(';')))
                {
                    if (data)
                    {
                        DataSet ds = new DataSet();
                        string[] fields_ = fields.Split(';');
                        if (this.SQLQuery(ref ds, "SELECT * FROM " + tab.TableName, tab.TableName, true))
                        {
                            foreach (DataRow row in tab.Rows)
                            {
                                DataRow newRow = ds.Tables[0].NewRow();
                                foreach (string field in fields_)
                                {
                                    newRow[field] = row[field];
                                }
                                ds.Tables[0].Rows.Add(newRow);
                            }
                            if (!this.UpdateData(ref ds, tab.TableName)) return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                m_errMsg = e.Message;
                _lastException = e;
                return false;
            }
            //return true;
        }

        public dataType getFieldType(string fieldname)
        {
            if (_schemaTable == null) return dataType.unknown;
            DataRow[] row = _schemaTable.Select("ColumnName='" + fieldname + "'");
            if (row.Length == 0) return dataType.unknown;
            if (row[0]["ProviderType"] == null) return dataType.unknown;

            int type = Convert.ToInt32(row[0]["ProviderType"]);
            switch (Convert.ToInt32(row[0]["ProviderType"]))
            {
                case 3: return dataType.integer;
                case 2: return dataType.boolean;  // eigentlich Bit (ja/nein) bei SQL-Server
                case 202: return dataType.text;
                case 5: return dataType.real;
                case 4: return dataType.date;  // bei SQL-Server ???
                case 7: return dataType.date;
                case 6: return dataType.currency;
                case 11: return dataType.boolean;
            }
            return dataType.text;
        }
        public int getFieldSize(string fieldname)
        {
            if (_schemaTable == null) return 0;
            DataRow[] row = _schemaTable.Select("ColumnName='" + fieldname + "'");
            if (row.Length == 0) return 0;
            if (row[0]["ColumnSize"] == null) return 0;

            int len = Convert.ToInt32(row[0]["ColumnSize"]);
            return len;
        }
        // Nur zum Testen... 
        public string schemaString
        {
            get
            {
                if (_schemaTable == null) return "";
                DataRow[] rows = _schemaTable.Select();
                string ret = "";
                foreach (DataRow row in rows)
                {
                    ret += row["ColumnName"].ToString() + ": ProviderType=" + row["ProviderType"].ToString() + "\n";
                }
                return ret;
            }
        }
        public DataTable schemaTable
        {
            get
            {
                return _schemaTable;
            }
        }
        static public string providedDBTypes
        {
            get
            {
                return "Odbc,OleDb,Sql";
            }
        }

        protected string mdbPath
        {
            get
            {
                if (_dbtype != DBType.oledb) return "";
                int pos1 = _connectionString.IndexOf("Data Source=");
                if (pos1 == -1) return "";
                int pos2 = _connectionString.IndexOf(";", pos1);
                if (pos2 == -1) pos2 = _connectionString.Length - 1;

                return _connectionString.Substring(pos1 + 12, pos2 - pos1 - 11);
            }
        }
        public bool CompactAccessDB()
        {
            m_errMsg = "Not Implementet";
            return false;

            //
            // gView.DB sollte sich im GAC befinden
            // Darum keine Wrapper auf JRO,... einbinden...
            //

            /*
			if(m_dbtype!=DBType.oledb) return false;

			string target="C:\\tempdb"+System.Guid.NewGuid().ToString("N")+".mdb";

			JRO.JetEngine engine=new JRO.JetEngineClass();
			engine.CompactDatabase(m_connStr,"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+target+";Jet OLEDB:Engine Type=5");
			
			System.IO.File.Delete(mdbPath);
			System.IO.File.Move(target,mdbPath);

		    System.Runtime.InteropServices.Marshal.ReleaseComObject(engine);
			engine=null;
			return true;
             * */
        }

        public string[] TableNames()
        {
            List<string> tableNames = new List<string>();
            try
            {
                switch (dbType)
                {
                    case DBType.oledb:
                        using (OleDbConnection connection = new OleDbConnection(this.ConnectionString))
                        {
                            connection.Open();
                            DataTable tables = connection.GetSchema("Tables");
                            foreach (DataRow row in tables.Rows)
                            {
                                string schema = row["TABLE_SCHEMA"].ToString();
                                string table = string.IsNullOrEmpty(schema) ? row["TABLE_NAME"].ToString() : schema + "." + row["TABLE_NAME"].ToString();
                                tableNames.Add(table);
                            }
                        }
                        break;
                    case DBType.sql:
                        using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                        {
                            connection.Open();
                            DataTable tables = connection.GetSchema("Tables");
                            foreach (DataRow row in tables.Rows)
                            {
                                string schema = row["TABLE_SCHEMA"].ToString();
                                string table = string.IsNullOrEmpty(schema) ? row["TABLE_NAME"].ToString() : schema + "." + row["TABLE_NAME"].ToString();
                                tableNames.Add(table);
                            }
                        }
                        break;
                    case DBType.npgsql:
                        DbProviderFactory dbfactory = DataProvider.PostgresProvider;
                        using (DbConnection dbconnection = dbfactory.CreateConnection())
                        {
                            dbconnection.ConnectionString = this.ConnectionString;
                            DataTable tables3 = dbconnection.GetSchema("Tables");
                            foreach (DataRow row in tables3.Rows)
                            {
                                string schema = row["table_schema"].ToString();
                                string table = string.IsNullOrEmpty(schema) ? row["table_name"].ToString() : schema + "." + row["table_name"].ToString();
                                tableNames.Add(table);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                m_errMsg = ex.Message;
                _lastException = ex;
            }
            return tableNames.ToArray();
        }
    }
}
