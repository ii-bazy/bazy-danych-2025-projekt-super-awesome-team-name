import matplotlib.pyplot as plt
from db import get_dynamic_data


class Plotter:
    @staticmethod
    def plot_dynamic(ax, plot_type, x_choice, y_choice, date_from=None, date_to=None):
        data = get_dynamic_data(x_choice, y_choice, date_from, date_to)

        if not data:
            ax.text(0.5, 0.5, "Brak danych do wyświetlenia", ha='center', va='center', transform=ax.transAxes)
            ax.set_title("Brak danych")
            plt.tight_layout()
            return

        sorted_data = sorted(data, key=lambda item: item[0])
        labels, values = zip(*sorted_data)

        y_min = min(values)
        y_max = max(values)

        ylim_bottom = max(0, y_min - 1)
        ylim_top = y_max + 1

        ax.set_ylabel(y_choice)
        ax.set_xlabel(x_choice)

        if plot_type == "Słupkowy":
            ax.bar(range(len(labels)), values)
            ax.set_ylim(bottom=ylim_bottom, top=ylim_top)

            ax.set_xticks(range(len(labels)))
            ax.set_xticklabels(labels, rotation=45, ha='right')

            if len(labels) > 10:
                ax.xaxis.set_major_locator(plt.MaxNLocator(nbins=10))
            plt.setp(ax.get_xticklabels(), ha="right")
            ax.tick_params(axis='x', rotation=45)

        else:
            ax.text(0.5, 0.5, "Nieznany typ wykresu", ha='center', va='center', transform=ax.transAxes)
            ax.set_title("Błąd")
            plt.tight_layout()
            return

        plt.tight_layout()
