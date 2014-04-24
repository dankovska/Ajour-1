using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Abstract
{
    public enum MessageType
    {
        UnknownType,

        ADMConfirmsPlannedOrRegisteredToBTM,
        ADMConfirmsPlannedOrRegisteredToDIR,
        ADMConfirmsPlannedOrRegisteredToEMP,
        ADMConfirmsPlannedOrRegisteredToACC,
 
        ADMRegistersPlannedOrPlannedModifiedToBTM,
        ADMRegistersPlannedOrPlannedModifiedToEMP,
        ADMRegistersPlannedOrPlannedModifiedToACC,


        ADMReplansRegisteredOrRegisteredModifiedToBTM,
        ADMReplansRegisteredOrRegisteredModifiedToACC,


        ADMCancelsRegisteredOrRegisteredModifiedToBTM,
        ADMCancelsRegisteredOrRegisteredModifiedToEMP,
        ADMCancelsRegisteredOrRegisteredModifiedToACC,

        ADMCancelsConfirmedOrConfirmedModifiedToBTM,
        ADMCancelsConfirmedOrConfirmedModifiedToDIR,
        ADMCancelsConfirmedOrConfirmedModifiedToEMP,
        ADMCancelsConfirmedOrConfirmedModifiedToACC,

        BTMUpdatesConfirmedOrConfirmedModifiedToEMP,
        BTMUpdatesConfirmedOrConfirmedModifiedToACC,
        BTMUpdateVisaRegistrationDateToEMP,
        BTMCreateVisaRegistrationDateToEMP,

        BTMReportsConfirmedOrConfirmedModifiedToACC,
        BTMReportsConfirmedOrConfirmedModifiedToEMP,

        BTMRejectsRegisteredOrRegisteredModifiedToADM,
        BTMRejectsRegisteredOrRegisteredModifiedToACC,

        BTMRejectsConfirmedOrConfirmedModifiedToADM,
        BTMRejectsConfirmedOrConfirmedModifiedToEMP,
        BTMRejectsConfirmedOrConfirmedModifiedToACC,

        ACCCancelsConfirmedReportedToADM,
        ACCCancelsConfirmedReportedToBTM,
        ACCCancelsConfirmedReportedToEMP,

        ACCModifiesConfirmedReportedToADM,
        ACCModifiesConfirmedReportedToBTM,
        ACCModifiesConfirmedReportedToDIR,
        ACCModifiesConfirmedReportedToEMP, 

        DIRRejectsConfirmedToADM,
        DIRRejectsConfirmedToEMP,
        DIRRejectsConfirmedToBTM,
        DIRRejectsConfirmedToACC,

        BTMCancelsPermitToADM,

        ADMCancelsPlannedModifiedToBTM,
        ADMCancelsPlannedModifiedToACC,

        ADMConfirmsPlannedOrRegisteredToResponsibleInLocation,
        ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation,
        ACCCancelsConfirmedReportedToResponsibleInLocation,
        BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation,
        DIRRejectsConfirmedToResponsibleInLocation
    }
}
