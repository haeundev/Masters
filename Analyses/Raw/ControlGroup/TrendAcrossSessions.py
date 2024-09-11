import os
from ParticipantDataProcessor import ParticipantDataProcessor

# get xlsx files from dir
xlsx_files = [f for f in os.listdir(f'{os.getcwd()}') if f.endswith('.xlsx')]

for file in xlsx_files:
    pid = file.split('.')[0]
    processor = ParticipantDataProcessor(pid)

    session_accuracies = processor.get_accuracy_average(processor.get_valid_sessions([processor.get_sheet(f"Session {i + 1}", "training") for i in range(8)]))
    result = {f"Session {i + 1}": session_accuracies[i] for i in range(len(session_accuracies))}

    break