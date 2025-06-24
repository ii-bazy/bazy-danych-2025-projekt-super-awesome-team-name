from db import get_sales_data, get_dynamic_data

class Plotter:
    def plot_dynamic(self, ax, plot_type, x_choice, y_choice):
        data = get_dynamic_data(x_choice, y_choice)

        if not data:
            ax.text(0.5, 0.5, "Brak danych do wyświetlenia", ha='center', va='center')
            return

        labels, values = zip(*data)

        if plot_type == "Słupkowy":
            ax.bar(labels, values)
        elif plot_type == "Liniowy":
            ax.plot(labels, values, marker='o')
        elif plot_type == "Kołowy":
            ax.pie(values, labels=labels, autopct='%1.1f%%')
        else:
            ax.text(0.5, 0.5, "Nieznany typ wykresu", ha='center', va='center')

    def plot_example(self, ax, plot_type):
        data = get_sales_data()

        if not data:
            ax.text(0.5, 0.5, "Brak danych do wyświetlenia", ha='center', va='center')
            return

        labels, values = zip(*data)  # rozdziela [(nazwa, ilość), ...] na 2 listy

        if plot_type == "Słupkowy":
            ax.bar(labels, values)
            ax.set_title("Liczba sprzedanych produktów")
        elif plot_type == "Liniowy":
            ax.plot(labels, values, marker='o')
            ax.set_title("Liczba sprzedanych produktów (wykres liniowy)")
        elif plot_type == "Kołowy":
            ax.pie(values, labels=labels, autopct='%1.1f%%')
            ax.set_title("Procentowy udział produktów w sprzedaży")
        else:
            ax.text(0.5, 0.5, "Nieznany typ wykresu", ha='center', va='center')
