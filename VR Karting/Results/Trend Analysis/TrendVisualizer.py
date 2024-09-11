import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

# Load the data from an Excel file
df = pd.read_excel('TrendData.xlsx')

# Set a larger color palette using seaborn's color_palette function
# You can change "tab20" to any other seaborn palette or define your own list of colors
colors = sns.color_palette("tab20", n_colors=df['ParticipantID'].nunique())

# Plot each group's participant score trends in separate figures and save them
group_colors = {}  # To store unique colors per group
for (group, sub_df) in df.groupby('Group'):
    fig, ax = plt.subplots(figsize=(12, 6))  # Adjust figure size as needed
    color_index = 0  # Reset color index for each group
    for _, row in sub_df.iterrows():
        # Ensure each participant has a unique color
        if group not in group_colors:
            group_colors[group] = {}
        if row['ParticipantID'] not in group_colors[group]:
            group_colors[group][row['ParticipantID']] = colors[color_index]
            color_index += 1

        ax.plot(range(1, 9), row[['Session 1', 'Session 2', 'Session 3', 'Session 4',
                                  'Session 5', 'Session 6', 'Session 7', 'Session 8']],
                marker='o', label=f"ID {row['ParticipantID']}", color=group_colors[group][row['ParticipantID']])
    ax.set_title(f'Participant Score Trend in {group}')
    ax.set_xlabel('Sessions')
    ax.set_ylabel('Scores')
    ax.legend(bbox_to_anchor=(1.05, 1), loc='upper left')
    plt.tight_layout()  # Adjust layout to make room for legend
    plt.savefig(f'{group}_Scores_Trend.png', dpi=300,
                bbox_inches='tight')  # Save with bounding box tightly around the figure
    plt.show()

# Plot average scores for each group across sessions and save it
fig, ax = plt.subplots(figsize=(12, 6))
for idx, (group, sub_df) in enumerate(df.groupby('Group')):
    session_means = sub_df[[f"Session {i}" for i in range(1, 9)]].mean()
    # Use distinct colors for each group in the average plot
    ax.plot(range(1, 9), session_means, marker='o', label=f"{group}", color=colors[idx])
ax.set_title('Average Group Scores Across Sessions')
ax.set_xlabel('Sessions')
ax.set_ylabel('Average Score')
ax
