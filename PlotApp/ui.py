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
        top_frame.pack(side=tk.TOP, fill=tk.X)

        # Typ wykresu
        self.plot_type = ttk.Combobox(top_frame, values=["Słupkowy", "Liniowy", "Kołowy"], width=10)
        self.plot_type.set("Słupkowy")
        self.plot_type.pack(side=tk.LEFT, padx=5, pady=5)

        # Oś X
        self.x_axis = ttk.Combobox(
            top_frame,
            values=["Produkt", "Użytkownik", "Data"],
            width=15
        )
        self.x_axis.set("Produkt")
        self.x_axis.pack(side=tk.LEFT, padx=5, pady=5)

        # Oś Y
        self.y_axis = ttk.Combobox(
            top_frame,
            values=["Liczba zamówień", "Liczba sztuk", "Suma wydatków"],
            width=20
        )
        self.y_axis.set("Liczba sztuk")
        self.y_axis.pack(side=tk.LEFT, padx=5, pady=5)

        # Przycisk generujący wykres
        plot_button = ttk.Button(top_frame, text="Generuj wykres", command=self.update_plot)
        plot_button.pack(side=tk.LEFT, padx=5)

        # Obszar na wykres
        self.figure = Figure(figsize=(10, 6), dpi=100)
        self.ax = self.figure.add_subplot(111)
        self.canvas = FigureCanvasTkAgg(self.figure, master=self.root)
        self.canvas.get_tk_widget().pack(fill=tk.BOTH, expand=True)

    def update_plot(self):
        self.ax.clear()
        plot_type = self.plot_type.get()
        x_choice = self.x_axis.get()
        y_choice = self.y_axis.get()
        self.plotter.plot_dynamic(self.ax, plot_type, x_choice, y_choice)
        self.canvas.draw()

    def run(self):
        self.root.mainloop()
