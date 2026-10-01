import mssql_python

if __name__ == '__main__':
    connection_string = "SERVER=COMP11A1\\SQLEXPRESS;DATABASE=DB1;Trusted_Connection=yes;Encrypt=yes;TrustServerCertificate=yes;"
    connection = mssql_python.connect(connection_string)

    cursor = connection.cursor()
    cursor.execute("""
        SELECT TOP 10 name, list_price
        FROM dbo.products
    """)
    rows = cursor.fetchall()

    for row in rows:
        print(row)

    connection.close()
    