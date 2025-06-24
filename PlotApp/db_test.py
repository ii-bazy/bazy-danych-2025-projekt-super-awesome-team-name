from db import get_sales_data

def main():
    try:
        data = get_sales_data()
        print("Dane sprzedaży:")
        for name, total in data:
            print(f"{name}: {total}")
    except Exception as e:
        print("Błąd:", e)

if __name__ == "__main__":
    main()
