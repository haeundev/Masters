import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
from scipy.stats import linregress

def analyze_and_plot(df, dataset_name):
    # Melting the DataFrame for easier plotting
    df_melted = df.reset_index().melt(id_vars=["Participant"], var_name="Session", value_name="Score")

    # Plotting the data for each participant
    plt.figure(figsize=(12, 8))
    plot = sns.lineplot(data=df_melted, x="Session", y="Score", hue="Participant", marker="o", palette="tab10")
    plt.title(f"Scores Across Training Sessions by Participant - {dataset_name}")
    plt.xlabel("Training Session")
    plt.ylabel("Score")
    plt.legend(title="Participant ID", bbox_to_anchor=(1.05, 1), loc='upper left')
    plt.grid(True)
    plt.xticks(rotation=45)
    plt.tight_layout()
    plt.show()
    # Save the plot as a png file
    file_name = f'scores_by_participant_{dataset_name.replace(" ", "_")}'
    plot.get_figure().savefig(f'{file_name}.png')

    # Calculate average scores per session
    average_scores = df.mean()

    # Linear regression on the average scores
    sessions = np.arange(1, len(average_scores) + 1)
    slope, intercept, r_value, p_value, std_err = linregress(sessions, average_scores)

    # Plotting average scores with trend line
    plt.figure(figsize=(8, 5))
    plot = sns.lineplot(x=sessions, y=average_scores, marker="o", label="Average Scores")
    plt.plot(sessions, intercept + slope * sessions, 'r', label=f'Trend Line (slope={slope:.2f})')
    plt.title("Average Scores Across Sessions with Trend Line")
    plt.xlabel("Session Number")
    plt.ylabel("Average Score")
    plt.legend()
    plt.text(0.5, 0.9, f'P-value: {p_value:.4f}\nR-squared: {r_value**2:.4f}',
             ha='center', va='center', transform=plt.gca().transAxes, fontsize=12)
    plt.grid(True)
    plt.tight_layout()
    plt.show()

    # Save the plot as a png file
    file_name = f'average_scores_{dataset_name.replace(" ", "_")}'
    plot.get_figure().savefig(f'{file_name}.png')

    # Print regression results
    print(f"Slope: {slope:.2f}")
    print(f"Intercept: {intercept:.2f}")
    print(f"P-value: {p_value:.4f}")
    print(f"R-squared: {r_value**2:.4f}")

# Reading data from Excel
excel_file = 'TrendData.xlsx'
data1 = pd.read_excel(excel_file, sheet_name='Training', index_col='Participant')
data2 = pd.read_excel(excel_file, sheet_name='Evaluation', index_col='Participant')

print("Analysis for Dataset 1 (Training):")
analyze_and_plot(data1, "Training")

print("Analysis for Dataset 2 (Evaluation):")
analyze_and_plot(data2, "Evaluation")
