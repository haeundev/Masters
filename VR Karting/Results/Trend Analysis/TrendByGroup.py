import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

# Load the data from an Excel file
file_path = 'TrendData.xlsx'  # Replace with your actual file path
data = pd.read_excel(file_path)

# Melt the data to long format
melted_data = pd.melt(data, id_vars=['Group', 'ParticipantID'],
                      value_vars=[f'Session {i}' for i in range(1, 9)],
                      var_name='Session', value_name='Score')

# Convert session labels to numeric
melted_data['Session_Num'] = melted_data['Session'].str.extract('(\d+)').astype(int)

# Calculate mean and standard error for each group and session
grouped_data = melted_data.groupby(['Group', 'Session_Num']).agg(
    Mean_Score=('Score', 'mean'),
    Std_Error=('Score', lambda x: np.std(x, ddof=1) / np.sqrt(len(x)))
).reset_index()

# Plotting for each session and group
plt.figure(figsize=(10, 6))

# Define line styles and markers for each group
group_styles = {
    'Control': {'color': 'lightseagreen', 'linestyle': '-', 'marker': 'o'},
    'DriveOnly': {'color': 'orange', 'linestyle': '--', 'marker': '^'},
    'NoiseDrive': {'color': 'slateblue', 'linestyle': '-.', 'marker': 's'}
}

# Plot each group with error bars for each session
for group in grouped_data['Group'].unique():
    group_data = grouped_data[grouped_data['Group'] == group]
    plt.errorbar(
        group_data['Session_Num'], group_data['Mean_Score'],
        yerr=group_data['Std_Error'],
        label=group,
        color=group_styles[group]['color'],
        linestyle=group_styles[group]['linestyle'],
        marker=group_styles[group]['marker'],
        capsize=4
    )

# Customize plot
plt.axvline(x=4.5, color='black', linestyle='--', linewidth=1)  # Vertical line to split first 4 and last 4 sessions

# Titles and labels
plt.title('Trend of Scores Across Training Sessions with Group Differences')
plt.xlabel('Training Session')
plt.ylabel('Mean Score (with SE)')

# Custom legend
plt.legend(title='Group', loc='lower right')

# Add grid for better readability
plt.grid(True, linestyle='--', alpha=0.6)

# Save the plot as a PNG file
plt.tight_layout()
plt.savefig('training_sessions_trend.png', dpi=300)  # Save the plot as a PNG with high resolution

# Show the plot
plt.show()

# --- Additional Plot for Overall Trend (Average Across All Groups) ---
# Calculate overall mean and standard error across all groups
overall_trend = melted_data.groupby('Session_Num').agg(
    Mean_Score=('Score', 'mean'),
    Std_Error=('Score', lambda x: np.std(x, ddof=1) / np.sqrt(len(x)))
).reset_index()

# Plotting overall trend
plt.figure(figsize=(10, 6))

# Plot overall trend with error bars
plt.errorbar(
    overall_trend['Session_Num'], overall_trend['Mean_Score'],
    yerr=overall_trend['Std_Error'],
    label='Overall',
    color='royalblue',
    linestyle='-', marker='o', capsize=4
)

# Customize the plot
plt.axvline(x=4.5, color='black', linestyle='--', linewidth=1)  # Vertical line splitting first 4 and last 4 sessions

# Titles and labels
plt.title('Overall Trend of Scores Across Training Sessions')
plt.xlabel('Training Session')
plt.ylabel('Mean Score (with SE)')

# Add grid
plt.grid(True, linestyle='--', alpha=0.6)

# Save the plot as a PNG file
plt.tight_layout()
plt.savefig('overall_training_sessions_trend.png', dpi=300)  # Save the plot as a PNG with high resolution

# Show the plot
plt.show()