using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

[PublicAPI]
public class MostradoresFlags
{
    public bool Data { get; set; }
    public bool Hora { get; set; }
    public bool EnergiaAtivaDiretaGeral { get; set; }
    public bool EnergiaAtivaDiretaNoHorárioDaPonta { get; set; }
    public bool EnergiaAtivaDiretaNoHorárioReservado { get; set; }
    public bool EnergiaAtivaDiretaNoHorárioForaDaPonta { get; set; }
    public bool EnergiaAtivaDiretaNoHorárioIntermediário { get; set; }
    public bool DemandaMáximaDiretaNoHorárioDaPonta { get; set; }
    public bool DemandaMáximaDiretaNoHorárioReservado { get; set; }
    public bool DemandaMáximaDiretaNoHorárioForaDaPonta { get; set; }
    public bool DemandaMáximaDiretaNoHorárioIntermediário { get; set; }
    public bool DemandaAcumuladaDiretaNoHorárioDaPonta { get; set; }
    public bool DemandaAcumuladaDiretaNoHorárioReservado { get; set; }
    public bool DemandaAcumuladaDiretaNoHorárioForaDaPonta { get; set; }
    public bool DemandaAcumuladaDiretaNoHorárioIntermediário { get; set; }
    public bool NúmeroDeReposiçõesDeDemanda { get; set; }
    public bool EnergiaReativaPositivaOuIndutivaQ1Geral { get; set; }
    public bool EnergiaReativaPositivaOuIndutivaQ1NoHorárioDaPonta { get; set; }
    public bool EnergiaReativaPositivaOuIndutivaQ1NoHorárioReservado { get; set; }
    public bool EnergiaReativaPositivaOuIndutivaQ1NoHorárioForaDaPonta { get; set; }
    public bool EnergiaReativaPositivaOuIndutivaQ1NoHorárioIntermediário { get; set; }
    public bool EnergiaReativaNegativaOuCapacitivaQ4Geral { get; set; }
    public bool NúmeroDeSérieDoMedidor { get; set; }
    public bool DemandaMáximaDiretaGeral { get; set; }
    public bool DemandaAcumuladaDiretaGeral { get; set; }
    public bool EnergiaReativaNegativaOuCapacitivaQ4NoHorárioDaPonta { get; set; }
    public bool EnergiaReativaNegativaOuCapacitivaQ4NoHorárioReservado { get; set; }
    public bool EnergiaReativaNegativaOuCapacitivaQNoHorárioForaDaPonta { get; set; }
    public bool EnergiaAtivaReversaGeral { get; set; }
    public bool EnergiaAtivaReversaNoHorárioDaPonta { get; set; }
    public bool EnergiaAtivaReversaNoHorárioReservado { get; set; }
    public bool EnergiaAtivaReversaNoHorárioForaDaPonta { get; set; }
    public bool EnergiaAtivaReversaNoHorárioIntermediário { get; set; }
    public bool NúmeroDaUnidadeConsumidora { get; set; }
    public bool AutoTesteDoMostrador { get; set; }
    public bool PhPostoHorárioCorrente { get; set; }
    public bool CalibraçãoDi { get; set; }
    public bool UferTotal { get; set; }
    public bool UferNoHorárioDaPonta { get; set; }
    public bool UferNoHorárioReservado { get; set; }
    public bool UferNoHorárioForaDaPonta { get; set; }
    public bool DmcrNoHorárioDaPonta { get; set; }
    public bool DmcrNoHorárioReservado { get; set; }
    public bool DmcrNoHorárioForaDaPonta { get; set; }
    public bool DmcrAcumuladaNoHorárioDaPonta { get; set; }
    public bool DmcrAcumuladaNoHorárioReservado { get; set; }
    public bool DmcrAcumuladaNoHorárioForaDaPonta { get; set; }
    public bool DmcrGeral { get; set; }
    public bool DmcrAcumuladaGeral { get; set; }
    public bool DemandaMáximaReversaNoHorárioDaPonta { get; set; }
    public bool DemandaMáximaReversaNoHorárioReservado { get; set; }
    public bool DemandaMáximaReversaNoHorárioForaDaPonta { get; set; }
    public bool DemandaMáximaReversaNoHorárioIntermediário { get; set; }
    public bool DemandaAcumulaDaReversaNoHorárioDaPonta { get; set; }
    public bool DemandaAcumuladaReversaNoHorárioReservado { get; set; }
    public bool DemandaAcumuladaReversaNoHorárioForaDaPonta { get; set; }
    public bool DemandaAcumuladaReversaNoHorárioIntermediário { get; set; }
    public bool EnergiaReativaIndutivaQ3Geral { get; set; }
    public bool EnergiaReativaIndutivaQ3NoHorárioDaPonta { get; set; }
    public bool EnergiaReativaIndutivaQ3NoHorárioReservado { get; set; }
    public bool EnergiaReativaIndutivaQ3NoHorárioForaDaPonta { get; set; }
    public bool EnergiaReativaIndutivaQ3NoHorárioIntermediário { get; set; }
    public bool EnergiaReativaCapacitivaQ2Geral { get; set; }
    public bool DemandaMáximaReversaGeral { get; set; }
    public bool DemandaAcumuladaReversaGeral { get; set; }
    public bool EnergiaReativaCapacitivaQ2NoHorárioDaPonta { get; set; }
    public bool EnergiaReativaCapacitivaQ2NoHorárioReservado { get; set; }
    public bool EnergiaReativaCapacitivaQ2NoHorárioForaDaPonta { get; set; }
    public bool Últimos12ValoresCalculadosDeDrpDrc { get; set; }
    public bool DemandaUltimoIntervaloIntegracao { get; set; }
}