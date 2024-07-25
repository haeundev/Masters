import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt

# Load data
file_path = 'TrendData.xlsx'
training_data = pd.read_excel(file_path, sheet_name='Training')
evaluation_data = pd.read_excel(file_path, sheet_name='Evaluation')

# Melt the data for better handling with seaborn
training_melted = pd.melt(training_data, id_vars=['IsAutoDrive', 'NoiseType', 'Participant'],
                          value_vars=[f'Session {i}' for i in range(1, 9)],
                          var_name='Session', value_name='Score')
evaluation_melted = pd.melt(evaluation_data, id_vars=['IsAutoDrive', 'NoiseType', 'Participant'],
                            value_vars=[f'Session {i}' for i in range(1, 4)],
                            var_name='Session', value_name='Score')

# Function to map noise type to understandable labels
def map_noise_type(noise):
    if noise == 0:
        return 'No Noise'
    elif noise == 1:
        return 'Environmental-SingleTalker'
    elif noise == 2:
        return 'SingleTalker-Environmental'

training_melted['NoiseType'] = training_melted['NoiseType'].apply(map_noise_type)
evaluation_melted['NoiseType'] = evaluation_melted['NoiseType'].apply(map_noise_type)

# Function to plot box plots
def plot_box(data, x, y, hue, title, filename):
    plt.figure(figsize=(10, 6))
    sns.boxplot(data=data, x=x, y=y, hue=hue)
    plt.title(title)
    plt.legend(title=hue, bbox_to_anchor=(1.05, 1), loc='upper left')
    plt.tight_layout()
    plt.savefig(filename)
    plt.show()

# Comparison 1: Training sessions between auto drive group, drive group without noise, and drive group with noise
plot_box(training_melted, 'Session', 'Score', 'IsAutoDrive',
         'Training Scores: Auto Drive vs. Drive',
         'training_autodrive_vs_drive.png')

# Comparison 2: Evaluation sessions between auto drive group, drive group without noise, and drive group with noise
plot_box(evaluation_melted, 'Session', 'Score', 'IsAutoDrive',
         'Evaluation Scores: Auto Drive vs. Drive',
         'evaluation_autodrive_vs_drive.png')

# Comparison 3: Training sessions between drive group without noise and drive group with noise
training_no_auto = training_melted[training_melted['IsAutoDrive'] == 0]
plot_box(training_no_auto, 'Session', 'Score', 'NoiseType',
         'Training Scores: No Noise vs. ES vs. SE',
         'training_noise_conditions.png')

# Comparison 4: Evaluation sessions between drive group without noise and drive group with noise
evaluation_no_auto = evaluation_melted[evaluation_melted['IsAutoDrive'] == 0]
plot_box(evaluation_no_auto, 'Session', 'Score', 'NoiseType',
         'Evaluation Scores: No Noise vs. ES vs. SE',
         'evaluation_noise_conditions.png')
