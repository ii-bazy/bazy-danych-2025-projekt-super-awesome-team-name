import pyodbc

# Konfiguracja połączenia – dostosuj do siebie
SERVER = 'localhost\\SQLEXPRESS'  # lub nazwa instancji
DATABASE = 'bd_project'
DRIVER = 'ODBC Driver 17 for SQL Server'  # upewnij się, że masz ten sterownik

def get_connection():
    try:
        conn = pyodbc.connect(
            f"DRIVER={{{DRIVER}}};SERVER={SERVER};DATABASE={DATABASE};Trusted_Connection=yes;"
        )
        return conn
    except Exception as e:
        print("Błąd połączenia z bazą danych:", e)
        return None

def get_sales_data():
    """
    Przykładowe dane: łączna sprzedaż produktów.
    Zwraca listę krotek (nazwa produktu, sprzedana ilość)
    """
    conn = get_connection()
    if not conn:
        return []

    query = """
        SELECT p.name, SUM(oi.quantity) AS total_sold
        FROM OrderItems oi
        JOIN Products p ON oi.product_id = p.id
        GROUP BY p.name
        ORDER BY total_sold DESC;
    """

    try:
        cursor = conn.cursor()
        cursor.execute(query)
        results = cursor.fetchall()
        return [(row[0], row[1]) for row in results]
    except Exception as e:
        print("Błąd podczas pobierania danych:", e)
        return []
    finally:
        conn.close()

def get_dynamic_data(x_choice, y_choice):
    conn = get_connection()
    if not conn:
        return []

    # Mapowanie wyborów na kolumny i agregacje
    x_map = {
        "Produkt": "p.name",
        "Użytkownik": "u.username",
        "Data": "CONVERT(date, og.order_date)"
    }

    y_map = {
        "Liczba zamówień": "COUNT(DISTINCT og.id)",
        "Liczba sztuk": "SUM(oi.quantity)",
        "Suma wydatków": "SUM(oi.quantity * p.price)"
    }

    x = x_map.get(x_choice)
    y = y_map.get(y_choice)

    if not x or not y:
        return []

    query = f"""
        SELECT {x} AS label, {y} AS value
        FROM OrderItems oi
        JOIN OrderGroups og ON oi.order_group_id = og.id
        JOIN Users u ON og.user_id = u.id
        JOIN Products p ON oi.product_id = p.id
        GROUP BY {x}
        ORDER BY value DESC
    """

    try:
        cursor = conn.cursor()
        cursor.execute(query)
        results = cursor.fetchall()
        return [(str(row[0]), row[1]) for row in results]
    except Exception as e:
        print("Błąd podczas dynamicznego zapytania:", e)
        return []
    finally:
        conn.close()
