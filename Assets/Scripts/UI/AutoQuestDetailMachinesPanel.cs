using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotFramework.AutoQuest;

public class AutoQuestDetailMachinesPanel : MonoBehaviour {

    public enum AutoQuestDetailMachinePanelType {
        None,
        ThreeMachines,
        TwoMachines
    }

    public AutoQuestDetailMachinePanelType machinePanelType;

    public List<GameObject> machineRelationshipAndGameObjectsList;
    public List<GameObject> machineRelationshipOrGameObjectsList;
    public List<AutoQuestDetailMachine> machinesList;

    public AutoQuest Quest { get; set; }
    public AutoQuestTerm QuestTerm { get; set; }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Refresh(AutoQuest quest, AutoQuestTerm questTerm) {
        Quest = quest;
        QuestTerm = questTerm;

        if (machineRelationshipAndGameObjectsList != null) {
            bool bShowMachineAndRelationship = quest.MachinesChosen.MachineRelationship == AutoQuestMachineRelationship.And;
            for (int i = 0; i < machineRelationshipAndGameObjectsList.Count; i++) {
                machineRelationshipAndGameObjectsList[i].gameObject.SetActive(bShowMachineAndRelationship);
            }
        }

        if (machineRelationshipOrGameObjectsList != null) {
            bool bShowMachineOrRelationship = quest.MachinesChosen.MachineRelationship == AutoQuestMachineRelationship.Or;
            for (int i = 0; i < machineRelationshipOrGameObjectsList.Count; i++) {
                machineRelationshipOrGameObjectsList[i].gameObject.SetActive(bShowMachineOrRelationship);
            }
        }

        if (machinesList != null) {
            int machinesCount = Mathf.Min(machinesList.Count, quest.MachinesChosen.MachineNamesList.Count);
            for (int i = 0; i < machinesCount; i++) {
                AutoQuestDetailMachine machine = machinesList[i];
                machine.Refresh(quest, QuestTerm, quest.MachinesChosen.MachineNamesList[i]);
            }
        }
    }

    public AutoQuestDetailMachine QuestDetailMachineByMachineName(string machineName) {
        for (int i = 0; i < machinesList.Count; i++) {
            AutoQuestDetailMachine machine = machinesList[i];
            if (machine.MachineName == machineName) {
                return machine;
            }
        }
        return null;
    }

    public void EnterMachine() {
        List<string> MachineNames = QuestTerm.MachineNamesUncompleted;
        AutoQuestDetailMachine firstMachine = null;
        bool machineEnter = false;
        for (int i = 0; i < MachineNames.Count; i++) {
            AutoQuestDetailMachine machine = QuestDetailMachineByMachineName(MachineNames[i]);
            if (machine != null) {
                if (firstMachine == null) {
                    firstMachine = machine;
                }

                if (machine.IsMachineReady) {
                    machine.EnterMachine();
                    machineEnter = true;
                    break;
                }
            }
        }

        if (firstMachine != null && machineEnter == false) {
            firstMachine.DownloadMachine();
        }
    }
}
