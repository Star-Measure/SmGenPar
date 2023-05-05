using System.Windows.Controls;
using SmGenPar.Logic.Models;

namespace SmGenPar.Wpf.MeterModels;

public partial class ParametrosExtend : UserControl
{
    readonly Extend _extend;
    public ParametrosExtend()
    {
        InitializeComponent();
        _extend = new();
        Load();
    }

    void Load()
    {
        //TabParametros.ItemsSource = Extend.ParameterNames;
        var postosHorarios = new PostosHorarios();
        PostosFrom.ItemsSource = postosHorarios;
    }
}