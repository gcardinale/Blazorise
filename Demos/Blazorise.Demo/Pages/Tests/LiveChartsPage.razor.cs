﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Charts;
using Blazorise.Charts.Streaming;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Demo.Pages.Tests
{
    public partial class LiveChartsPage : ComponentBase
    {
        private LineChart<LiveDataPoint> horizontalLineChart;
        private LineChart<LiveDataPoint> verticalLineChart;

        private BarChart<LiveDataPoint> horizontalBarChart;
        private HorizontalBarChart<LiveDataPoint> verticalBarChart;

        private Random random = new( DateTime.Now.Millisecond );

        private string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
        private List<string> backgroundColors = new() { ChartColor.FromRgba( 255, 99, 132, 0.5f ), ChartColor.FromRgba( 54, 162, 235, 0.5f ), ChartColor.FromRgba( 255, 206, 86, 0.5f ), ChartColor.FromRgba( 75, 192, 192, 0.5f ), ChartColor.FromRgba( 153, 102, 255, 0.5f ), ChartColor.FromRgba( 255, 159, 64, 0.5f ) };
        private List<string> borderColors = new() { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

        public struct LiveDataPoint
        {
            public object X { get; set; }

            public object Y { get; set; }
        }

        private object horizontalLineChartOptions = new
        {
            Title = new
            {
                Display = true,
                Text = "Line chart (horizontal scroll) sample"
            },
            Scales = new
            {
                YAxes = new object[]
                {
                    new {
                        ScaleLabel = new {
                        Display = true, LabelString = "value" }
                    }
                }
            },
            Tooltips = new
            {
                Mode = "nearest",
                Intersect = false
            },
            Hover = new
            {
                Mode = "nearest",
                Intersect = false
            }
        };

        private object verticalLineChartOptions = new
        {
            Title = new
            {
                Display = true,
                Text = "Line chart (vertical scroll) sample"
            },
            Scales = new
            {
                XAxes = new object[]
                {
                    new {
                        Type = "linear",
                        Display = true,
                        ScaleLabel = new {
                            Display = true, LabelString = "value"
                        }
                    }
                }
            },
            Tooltips = new
            {
                Mode = "nearest",
                Intersect = false
            },
            Hover = new
            {
                Mode = "nearest",
                Intersect = false
            }
        };

        private object horizontalBarChartOptions = new
        {
            Title = new
            {
                Display = true,
                Text = "Bar chart (horizontal scroll) sample"
            },
            Scales = new
            {
                YAxes = new object[]
                {
                    new
                    {
                        ScaleLabel = new
                        {
                            Display = true, LabelString = "value"
                        }
                    }
                }
            },
            Tooltips = new
            {
                Mode = "nearest",
                Intersect = false
            },
            Hover = new
            {
                Mode = "nearest",
                Intersect = false
            }
        };

        private object verticalBarChartOptions = new
        {
            Title = new
            {
                Display = true,
                Text = "Bar chart (vertical scroll) sample"
            },
            Scales = new
            {
                XAxes = new object[]
                {
                    new {
                        ScaleLabel = new
                        {
                            Display = true, LabelString = "value"
                        }
                    }
                }
            },
            Tooltips = new
            {
                Mode = "nearest",
                Intersect = false
            },
            Hover = new
            {
                Mode = "nearest",
                Intersect = false
            }
        };

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await Task.WhenAll(
                    HandleRedraw( horizontalLineChart, GetLineChartDataset1, GetLineChartDataset2 ),
                    HandleRedraw( verticalLineChart, GetLineChartDataset1, GetLineChartDataset2 ),
                    HandleRedraw( horizontalBarChart, GetBarChartDataset1 ),
                    HandleRedraw( verticalBarChart, GetBarChartDataset2 ) );
            }
        }

        private async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>( BaseChart<TDataSet, TItem, TOptions, TModel> chart, params Func<TDataSet>[] getDataSets )
            where TDataSet : ChartDataset<TItem>
            where TOptions : ChartOptions
            where TModel : ChartModel
        {
            await chart.Clear();

            await chart.AddLabelsDatasetsAndUpdate( Labels, getDataSets.Select( x => x.Invoke() ).ToArray() );
        }

        private async Task AddNewHorizontalLineDataSet()
        {
            var colorIndex = horizontalLineChart.Data.Datasets.Count % backgroundColors.Count;

            await horizontalLineChart.AddDatasetsAndUpdate( new LineChartDataset<LiveDataPoint>
            {
                Data = new(),
                Label = $"Dataset {horizontalLineChart.Data.Datasets.Count + 1}",
                BackgroundColor = backgroundColors[colorIndex],
                BorderColor = borderColors[colorIndex],
                Fill = false,
                LineTension = 0,
            } );
        }

        private async Task AddNewHorizontalLineData()
        {
            foreach ( var dataset in horizontalLineChart.Data.Datasets )
            {
                await horizontalLineChart.AddData( horizontalLineChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
                {
                    X = DateTime.Now,
                    Y = RandomScalingFactor(),
                } );
            }

            await horizontalLineChart.Update();
        }

        private async Task AddNewVerticalLineDataSet()
        {
            var colorIndex = verticalLineChart.Data.Datasets.Count % backgroundColors.Count;

            await verticalLineChart.AddDatasetsAndUpdate( new LineChartDataset<LiveDataPoint>
            {
                Data = new(),
                Label = $"Dataset {verticalLineChart.Data.Datasets.Count + 1}",
                BackgroundColor = backgroundColors[colorIndex],
                BorderColor = borderColors[colorIndex],
                Fill = false,
                LineTension = 0,
            } );
        }

        private async Task AddNewVerticalLineData()
        {
            foreach ( var dataset in verticalLineChart.Data.Datasets )
            {
                await verticalLineChart.AddData( verticalLineChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
                {
                    X = RandomScalingFactor(),
                    Y = DateTime.Now,
                } );
            }

            await verticalLineChart.Update();
        }

        private async Task AddNewHorizontalBarDataSet()
        {
            var colorIndex = horizontalBarChart.Data.Datasets.Count % backgroundColors.Count;

            await horizontalBarChart.AddDatasetsAndUpdate( new BarChartDataset<LiveDataPoint>
            {
                Data = new(),
                Label = $"Dataset {horizontalBarChart.Data.Datasets.Count + 1}",
                BackgroundColor = backgroundColors[colorIndex],
                BorderColor = borderColors[colorIndex],
            } );
        }

        private async Task AddNewHorizontalBarData()
        {
            foreach ( var dataset in horizontalBarChart.Data.Datasets )
            {
                await horizontalBarChart.AddData( horizontalBarChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
                {
                    X = DateTime.Now,
                    Y = RandomScalingFactor(),
                } );
            }

            await horizontalBarChart.Update();
        }

        private async Task AddNewVerticalBarDataSet()
        {
            var colorIndex = verticalBarChart.Data.Datasets.Count % backgroundColors.Count;

            await verticalBarChart.AddDatasetsAndUpdate( new BarChartDataset<LiveDataPoint>
            {
                Data = new(),
                Label = $"Dataset {verticalBarChart.Data.Datasets.Count + 1}",
                BackgroundColor = backgroundColors[colorIndex],
                BorderColor = borderColors[colorIndex],
            } );
        }

        private async Task AddNewVerticalBarData()
        {
            foreach ( var dataset in verticalBarChart.Data.Datasets )
            {
                await verticalBarChart.AddData( verticalBarChart.Data.Datasets.IndexOf( dataset ), new LiveDataPoint
                {
                    X = RandomScalingFactor(),
                    Y = DateTime.Now,
                } );
            }

            await verticalBarChart.Update();
        }

        private LineChartDataset<LiveDataPoint> GetLineChartDataset1()
        {
            return new()
            {
                Data = new(),
                Label = "Dataset 1 (linear interpolation)",
                BackgroundColor = backgroundColors[0],
                BorderColor = borderColors[0],
                Fill = false,
                LineTension = 0,
                BorderDash = new() { 8, 4 },
            };
        }

        private LineChartDataset<LiveDataPoint> GetLineChartDataset2()
        {
            return new()
            {
                Data = new(),
                Label = "Dataset 2 (cubic interpolation)",
                BackgroundColor = backgroundColors[1],
                BorderColor = borderColors[1],
                Fill = false,
                CubicInterpolationMode = "monotone",
            };
        }

        private BarChartDataset<LiveDataPoint> GetBarChartDataset1()
        {
            return new()
            {
                Data = new(),
                Label = "Dataset 1",
                BackgroundColor = backgroundColors[0],
                BorderColor = borderColors[0],
            };
        }

        private BarChartDataset<LiveDataPoint> GetBarChartDataset2()
        {
            return new()
            {
                Type = "horizontalBar",
                Data = new(),
                Label = "Dataset 1",
                BackgroundColor = backgroundColors[0],
                BorderColor = borderColors[0],
            };
        }

        private Task OnHorizontalLineRefreshed( ChartStreamingData<LiveDataPoint> data )
        {
            data.Value = new()
            {
                X = DateTime.Now,
                Y = RandomScalingFactor(),
            };

            return Task.CompletedTask;
        }

        private Task OnVerticalLineRefreshed( ChartStreamingData<LiveDataPoint> data )
        {
            data.Value = new()
            {
                X = RandomScalingFactor(),
                Y = DateTime.Now,
            };

            return Task.CompletedTask;
        }

        private Task OnHorizontalBarRefreshed( ChartStreamingData<LiveDataPoint> data )
        {
            data.Value = new()
            {
                X = DateTime.Now,
                Y = RandomScalingFactor(),
            };

            return Task.CompletedTask;
        }

        private Task OnVerticalBarRefreshed( ChartStreamingData<LiveDataPoint> data )
        {
            data.Value = new()
            {
                X = RandomScalingFactor(),
                Y = DateTime.Now,
            };

            return Task.CompletedTask;
        }

        private double RandomScalingFactor()
        {
            return ( random.NextDouble() > 0.5 ? 1.0 : -1.0 ) * Math.Round( random.NextDouble() * 100 );
        }
    }
}