import pyodbc

# Konfiguracja połączenia
SERVER = 'localhost\\SQLEXPRESS'  # lub nazwa instancji
DATABASE = 'bd_project'
DRIVER = 'ODBC Driver 17 for SQL Server'

def get_connection():
    try:
        conn = pyodbc.connect(
            f"DRIVER={{{DRIVER}}};SERVER={SERVER};DATABASE={DATABASE};Trusted_Connection=yes;"
        )
        return conn
    except Exception as e:
        print("Błąd połączenia z bazą danych:", e)
        return None

def get_sales_data(): # przykładowe dane do wykresu
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


def get_dynamic_data(x_choice, y_choice, date_from=None, date_to=None):
    conn = get_connection()
    if not conn:
        return []

    where_clauses = []
    params = []

    # Dodaj warunki filtrowania statusu
    where_clauses.append("og.status NOT IN (?, ?)")
    params.extend(['cart', 'cancelled'])

    if date_from:
        where_clauses.append("og.order_date >= ?")
        params.append(date_from)
    if date_to:
        where_clauses.append("og.order_date <= ?")
        params.append(date_to)

    where_sql = "WHERE " + " AND ".join(where_clauses) if where_clauses else ""

    x_map = {
        "Produkt": "p.name",
        "Użytkownik": "u.username",
        "Data": "CONVERT(date, og.order_date)"
    }

    y_map = {
        "Liczba zamówień": "COUNT(DISTINCT og.id)",
        "Liczba zamówionych sztuk": "SUM(oi.quantity)",
        "Suma wydatków": "SUM(oi.quantity * p.price)"
    }

    try:
        cursor = conn.cursor()

        x = x_map.get(x_choice)

        # Obsługa średnich dla dowolnego x_choice
        if y_choice == "Średnia liczba sztuk w zamówieniu":
            query = f"""
                SELECT {x} AS label,
                       CAST(SUM(oi.quantity) AS FLOAT) / COUNT(DISTINCT og.id) AS value
                FROM OrderItems oi
                JOIN OrderGroups og ON oi.order_group_id = og.id
                JOIN Users u ON og.user_id = u.id
                JOIN Products p ON oi.product_id = p.id
                {where_sql}
                GROUP BY {x}
                ORDER BY value DESC
            """
            cursor.execute(query, params)
            return [(str(row[0]), row[1]) for row in cursor.fetchall()]

        if y_choice == "Średnia wartość zamówienia":
            query = f"""
                SELECT {x} AS label,
                       CAST(SUM(oi.quantity * p.price) AS FLOAT) / COUNT(DISTINCT og.id) AS value
                FROM OrderItems oi
                JOIN OrderGroups og ON oi.order_group_id = og.id
                JOIN Users u ON og.user_id = u.id
                JOIN Products p ON oi.product_id = p.id
                {where_sql}
                GROUP BY {x}
                ORDER BY value DESC
            """
            cursor.execute(query, params)
            return [(str(row[0]), row[1]) for row in cursor.fetchall()]

        y = y_map.get(y_choice)
        if not x or not y:
            return []

        query = f"""
            SELECT {x} AS label, {y} AS value
            FROM OrderItems oi
            JOIN OrderGroups og ON oi.order_group_id = og.id
            JOIN Users u ON og.user_id = u.id
            JOIN Products p ON oi.product_id = p.id
            {where_sql}
            GROUP BY {x}
            ORDER BY value DESC
        """
        cursor.execute(query, params)
        return [(str(row[0]), row[1]) for row in cursor.fetchall()]

    except Exception as e:
        print("Błąd podczas dynamicznego zapytania:", e)
        return []
    finally:
        conn.close()