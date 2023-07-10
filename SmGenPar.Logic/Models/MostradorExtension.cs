using JetBrains.Annotations;

namespace SmGenPar.Logic.Models;

public static class MostradorExtension
{
    public static MostradoresFlag BitMapping(string code)
    {
        var index = code switch {
            "1" => 0,
            "2" => 1,
            "3" => 2,
            "4" => 3,
            "6" => 4,
            "8" => 5,
            "9" => 6,
            "10" => 7,
            "12" => 9,
            "14" => 10,
            "15" => 11,
            "17" => 12,
            "19" => 13,
            "21" => 14,
            "22" => 15,
            "23" => 16,
            "24" => 17,
            "25" => 18,
            "27" => 19,
            "29" => 20,
            "30" => 21,
            "31" => 22,
            "33" => 23,
            "52" => 24,
            "54" => 25,
            "85" => 26,
            "86" => 27,
            "87" => 28,
            "103" => 29,
            "104" => 30,
            "106" => 31,
            "108" => 32,
            "133" => 33,
            "188" => 34,
            "PH" => 35,
            "199" => 36,
            "65" => 37,
            "66" => 38,
            "67" => 39,
            "68" => 40,
            "69" => 41,
            "70" => 42,
            "71" => 43,
            "73" => 44,
            "74" => 45,
            "75" => 46,
            "78" => 47,
            "80" => 48,
            "110" => 49,
            "112" => 50,
            "114" => 51,
            "115" => 52,
            "117" => 53,
            "119" => 54,
            "121" => 55,
            "122" => 56,
            "124" => 57,
            "125" => 58,
            "127" => 59,
            "129" => 60,
            "130" => 61,
            "131" => 62,
            "152" => 63,
            "154" => 64,
            "185" => 65,
            "186" => 66,
            "187" => 67,
            "--" => 68,
            _ => throw new ArgumentException($"Invalid code: {code}.", nameof(code))
        };

        return(MostradoresFlag)index;
    }
    public static string CodeMapping(MostradoresFlag index)
    {
        string code = (int)index switch {
            0 => "1",
            1 => "2",
            2 => "3",
            3 => "4",
            4 => "6",
            5 => "8",
            6 => "9",
            7 => "10",
            9 => "12",
            10 => "14",
            11 => "15",
            12 => "17",
            13 => "19",
            14 => "21",
            15 => "22",
            16 => "23",
            17 => "24",
            18 => "25",
            19 => "27",
            20 => "29",
            21 => "30",
            22 => "31",
            23 => "33",
            24 => "52",
            25 => "54",
            26 => "85",
            27 => "86",
            28 => "87",
            29 => "103",
            30 => "104",
            31 => "106",
            32 => "108",
            33 => "133",
            34 => "188",
            35 => "PH",
            36 => "19",
            37 => "65",
            38 => "66",
            39 => "67",
            40 => "68",
            41 => "69",
            42 => "70",
            43 => "71",
            44 => "73",
            45 => "74",
            46 => "75",
            47 => "78",
            48 => "80",
            49 => "110",
            50 => "112",
            51 => "114",
            52 => "115",
            53 => "117",
            54 => "119",
            55 => "121",
            56 => "122",
            57 => "124",
            58 => "125",
            59 => "127",
            60 => "129",
            61 => "130",
            62 => "131",
            63 => "152",
            64 => "154",
            65 => "185",
            66 => "186",
            67 => "187",
            68 => "--",
            _ => throw new ArgumentException($"Invalid int {index}", nameof(index))
        };

        return code.ToString()!;
    }
}
[PublicAPI]
public enum MostradoresFlag
{
    Data                                                     = 0,
    Hora                                                     = 1,
    EnergiaAtivaDiretaGeral                                  = 2,
    EnergiaAtivaDiretaNoHorárioDaPonta                       = 3,
    EnergiaAtivaDiretaNoHorárioReservado                     = 4,
    EnergiaAtivaDiretaNoHorárioForaDaPonta                   = 5,
    EnergiaAtivaDiretaNoHorárioIntermediário                 = 6,
    DemandaMáximaDiretaNoHorárioDaPonta                      = 7,
    DemandaMáximaDiretaNoHorárioReservado                    = 8,
    DemandaMáximaDiretaNoHorárioForaDaPonta                  = 9,
    DemandaMáximaDiretaNoHorárioIntermediário                = 10,
    DemandaAcumuladaDiretaNoHorárioDaPonta                   = 11,
    DemandaAcumuladaDiretaNoHorárioReservado                 = 12,
    DemandaAcumuladaDiretaNoHorárioForaDaPonta               = 13,
    DemandaAcumuladaDiretaNoHorárioIntermediário             = 14,
    NúmeroDeReposiçõesDeDemanda                              = 15,
    EnergiaReativaPositivaOuIndutivaQ1Geral                  = 16,
    EnergiaReativaPositivaOuIndutivaQ1NoHorárioDaPonta       = 17,
    EnergiaReativaPositivaOuIndutivaQ1NoHorárioReservado     = 18,
    EnergiaReativaPositivaOuIndutivaQ1NoHorárioForaDaPonta   = 19,
    EnergiaReativaPositivaOuIndutivaQ1NoHorárioIntermediário = 20,
    EnergiaReativaNegativaOuCapacitivaQ4Geral                = 21,
    NúmeroDeSérieDoMedidor                                   = 22,
    DemandaMáximaDiretaGeral                                 = 23,
    DemandaAcumuladaDiretaGeral                              = 24,
    EnergiaReativaNegativaOuCapacitivaQ4NoHorárioDaPonta     = 25,
    EnergiaReativaNegativaOuCapacitivaQ4NoHorárioReservado   = 26,
    EnergiaReativaNegativaOuCapacitivaQNoHorárioForaDaPonta  = 27,
    EnergiaAtivaReversaGeral                                 = 28,
    EnergiaAtivaReversaNoHorárioDaPonta                      = 29,
    EnergiaAtivaReversaNoHorárioReservado                    = 30,
    EnergiaAtivaReversaNoHorárioForaDaPonta                  = 31,
    EnergiaAtivaReversaNoHorárioIntermediário                = 32,
    NúmeroDaUnidadeConsumidora                               = 33,
    AutoTesteDoMostrador                                     = 34,
    PhPostoHorárioCorrente                                   = 35,
    CalibraçãoDi                                             = 36,
    UferTotal                                                = 37,
    UferNoHorárioDaPonta                                     = 38,
    UferNoHorárioReservado                                   = 39,
    UferNoHorárioForaDaPonta                                 = 40,
    DmcrNoHorárioDaPonta                                     = 41,
    DmcrNoHorárioReservado                                   = 42,
    DmcrNoHorárioForaDaPonta                                 = 43,
    DmcrAcumuladaNoHorárioDaPonta                            = 44,
    DmcrAcumuladaNoHorárioReservado                          = 45,
    DmcrAcumuladaNoHorárioForaDaPonta                        = 46,
    DmcrGeral                                                = 47,
    DmcrAcumuladaGeral                                       = 48,
    DemandaMáximaReversaNoHorárioDaPonta                     = 49,
    DemandaMáximaReversaNoHorárioReservado                   = 50,
    DemandaMáximaReversaNoHorárioForaDaPonta                 = 51,
    DemandaMáximaReversaNoHorárioIntermediário               = 52,
    DemandaAcumulaDaReversaNoHorárioDaPonta                  = 53,
    DemandaAcumuladaReversaNoHorárioReservado                = 54,
    DemandaAcumuladaReversaNoHorárioForaDaPonta              = 55,
    DemandaAcumuladaReversaNoHorárioIntermediário            = 56,
    EnergiaReativaIndutivaQ3Geral                            = 57,
    EnergiaReativaIndutivaQ3NoHorárioDaPonta                 = 58,
    EnergiaReativaIndutivaQ3NoHorárioReservado               = 59,
    EnergiaReativaIndutivaQ3NoHorárioForaDaPonta             = 60,
    EnergiaReativaIndutivaQ3NoHorárioIntermediário           = 61,
    EnergiaReativaCapacitivaQ2Geral                          = 62,
    DemandaMáximaReversaGeral                                = 63,
    DemandaAcumuladaReversaGeral                             = 64,
    EnergiaReativaCapacitivaQ2NoHorárioDaPonta               = 65,
    EnergiaReativaCapacitivaQ2NoHorárioReservado             = 66,
    EnergiaReativaCapacitivaQ2NoHorárioForaDaPonta           = 67,
    Últimos12ValoresCalculadosDeDrpDrc                       = 68,
}