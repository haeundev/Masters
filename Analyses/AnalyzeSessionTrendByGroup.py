from Raw.ControlGroup.ParticipantDataProcessor import ParticipantDataProcessor
import os
from openpyxl import load_workbook

# get xlsx files from dir
control_group_xlsx_files = [f for f in os.listdir(f'{os.getcwd()}/Raw/ControlGroup')]

# open result file :
result_path = '/Users/forclass/Source/Masters/VR Karting/Results/Trend Analysis/TrendData.xlsx'
workbook = load_workbook(filename=result_path)
sheet = workbook.active  # Assumes the first sheet is the one you want to modify

for file in control_group_xlsx_files:
    participant_number = file.split('.')[0]
    p = ParticipantDataProcessor(participant_number)
    dfs_sessions = [p.get_sheet(f"Session {i + 1}", "evaluation") for i in range(9)]

    print(dfs_sessions)
    break