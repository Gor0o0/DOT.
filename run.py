from venv import create

import pyodbc

connection = pyodbc.connect(
    'DRIVER={ODBC Driver 17 for SQL Server};'
    'SERVER=COMP11A1\\SQLEXPRESS;'
    'DATABASE=DB1;'
    'Trusted_Connection=yes;'
    'TrustServerCertificate=yes;'
)

cursor = connection.cursor()

def get_name():
    table_names = ["customers","sales", "customer_products"]
    return table_names[number]

def get_date():
    query = [f"create table {get_name(2)} (id int, name varchar(50), list_price money)","INSERT INTO orders(quantity) VALUES(?)","SELECT TOP 50 id, quantity, name, list_price, created_at FROM orders"]
    return query[select]



# cursor.execute(get_date(0))
# # cursor.commit()

# cursor.execute(get_date(1), 3)
# # cursor.commit()

# cursor.execute(get_date(2))

def create_table(table_name: str):
    cursor.execute(get_date(0))
    cursor.commit()

def save_order(quantity: int):
    cursor.execute(get_date(1), quantity)
    cursor.commit()

def show_connection():
    cursor.execute(get_date(2))
    rows = cursor.fetchall()
    for row in rows:
        print(row)

def main():
    for i in range(2):
        name = get_date(int(get_name(i)))
        create_table(str(name))
        save_order(3)
        show_connection()
