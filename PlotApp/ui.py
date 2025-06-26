import tkinter as tk
from tkinter import ttk
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from matplotlib.figure import Figure
from plotter import Plotter


class SalesDashboard:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("Sales Dashboard")
        self.root.geometry("1000x700")

        self.plotter = Plotter()

        self.create_widgets()

    def create_widgets(self):
        # Górny pasek z przyciskami i wyborami
        top_frame = ttk.Frame(self.root)
        top_frame.pack(side=tk.TOP, fill=tk.X, padx=0, pady=(10, 5))

        # Typ wykresu
        self.plot_type = ttk.Combobox(top_frame, values=["Słupkowy", "Liniowy"], width=10)
        self.plot_type.set("Słupkowy")
        self.plot_type.pack(side=tk.LEFT, padx=5, pady=5)

        # Etykieta X
        x_label = ttk.Label(top_frame, text="Oś X:")
        x_label.pack(side=tk.LEFT, padx=(10, 2))

        self.x_axis = ttk.Combobox(
            top_frame,
            values=["Produkt", "Użytkownik", "Data"],
            width=15
        )
        self.x_axis.set("Produkt")
        self.x_axis.pack(side=tk.LEFT, padx=5, pady=5)

        # Etykieta Y
        y_label = ttk.Label(top_frame, text="Oś Y:")
        y_label.pack(side=tk.LEFT, padx=(10, 2))

        self.y_axis = ttk.Combobox(
            top_frame,
            values=[
                "Liczba zamówień",
                "Liczba zamówionych sztuk",
                "Suma wydatków",
                "Średnia liczba sztuk w zamówieniu",
                "Średnia wartość zamówienia"
            ],
            width=25
        )
        self.y_axis.set("Liczba zamówionych sztuk")
        self.y_axis.pack(side=tk.LEFT, padx=5, pady=5)

        self.y_axis.bind("<<ComboboxSelected>>", self.on_y_axis_change)

        # Zakres dat
        ttk.Label(top_frame, text="Od (YYYY-MM-DD):").pack(side=tk.LEFT, padx=(15, 2))
        self.date_from = ttk.Entry(top_frame, width=12)
        self.date_from.pack(side=tk.LEFT, padx=2)

        ttk.Label(top_frame, text="Do (YYYY-MM-DD):").pack(side=tk.LEFT, padx=(10, 2))
        self.date_to = ttk.Entry(top_frame, width=12)
        self.date_to.pack(side=tk.LEFT, padx=2)

        plot_button = ttk.Button(top_frame, text="Generuj wykres", command=self.update_plot)
        plot_button.pack(side=tk.LEFT, padx=5)

        self.widgets_to_toggle = [self.plot_type, self.x_axis]

        self.figure = Figure(figsize=(8, 5), dpi=100)
        self.ax = self.figure.add_subplot(111)
        self.canvas = FigureCanvasTkAgg(self.figure, master=self.root)


        self.canvas.get_tk_widget().pack(
            fill=tk.BOTH,
            expand=True,
            padx=0,
            pady=0
        )

    def update_plot(self):
        self.ax.clear()
        plot_type = self.plot_type.get()
        x_choice = self.x_axis.get()
        y_choice = self.y_axis.get()
        date_from = self.date_from.get()
        date_to = self.date_to.get()

        self.plotter.plot_dynamic(self.ax, plot_type, x_choice, y_choice, date_from, date_to)
        self.canvas.draw()

    def on_y_axis_change(self, event=None):
        for widget in self.widgets_to_toggle:
            widget.configure(state="normal")

    def run(self):
        self.root.mainloop()
