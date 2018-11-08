using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord;
using Accord.Statistics.Models.Regression.Linear;
using ZedGraph;
using Accord.Math.Optimization.Losses;

namespace 回归演示
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Declare some sample test data.
            double[] inputs = { 80, 60, 10, 20, 30 };
            double[] outputs = { 20, 40, 30, 50, 60 };

            //// Use Ordinary Least Squares to learn the regression
            //OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            //// Use OLS to learn the simple linear regression
            //SimpleLinearRegression regression = ols.Learn(inputs, outputs);

            //// Compute the output for a given input:
            ////double y = regression.Transform(85); // The answer will be 28.088

            //// We can also extract the slope and the intercept term
            //// for the line. Those will be -0.26 and 50.5, respectively.
            //double s = regression.Slope;     // -0.264706
            //double c = regression.Intercept; // 50.588235

            //double[] x= new double[20];
            //double[] y = new double[20];
            //for(int i = 0; i < 20; i++)
            //{
            //    x[i] = 5 + (i * 5);
            //    y[i] = s*x[i] + c;
            //}

            // We can create a learning algorithm
            var ls = new PolynomialLeastSquares()
            {
                Degree = 2
            };

            // Now, we can use the algorithm to learn a polynomial
            PolynomialRegression poly = ls.Learn(inputs, outputs);

            // The learned polynomial will be given by
            string str = poly.ToString("N1"); // "y(x) = 1.0x^2 + 0.0x^1 + 0.0"
            Console.WriteLine(str);
            // Where its weights can be accessed using
            double[] weights = poly.Weights;   // { 1.0000000000000024, -1.2407665029287351E-13 }
            double intercept = poly.Intercept; // 1.5652369518855253E-12
            Console.WriteLine("{0},{1}", weights[0],intercept);
            // Finally, we can use this polynomial
            // to predict values for the input data
            double[] pred = poly.Transform(inputs);
            double error = new SquareLoss(outputs).Loss(pred);
            Console.WriteLine(error);

            double[] x = new double[20];
            double[] y = new double[20];
            for (int i = 0; i < 20; i++)
            {
                x[i] = 5 + (i * 5);
                y[i] =weights[0]*x[i]*x[i]+weights[1]*x[i]+intercept;
            }
            GraphPane myPane = this.zedGraphControl1.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles
            myPane.Title.IsVisible = false;
            myPane.Chart.Border.IsVisible = false;
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Y";
            myPane.XAxis.IsAxisSegmentVisible = true;
            myPane.YAxis.IsAxisSegmentVisible = true;
            myPane.XAxis.MinorGrid.IsVisible = false;
            myPane.YAxis.MinorGrid.IsVisible = false;
            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.Scale.MinGrace = 0;
            myPane.XAxis.Scale.MaxGrace = 0;
            myPane.XAxis.Scale.Max = 90;
            //myPane.XAxis.Scale.Min = -10;
            myPane.YAxis.Scale.MinGrace = 0;
            myPane.YAxis.Scale.MaxGrace = 0;
            //myPane.YAxis.Scale.Min = -10;
            myPane.YAxis.Scale.Max = 70;

            PointPairList list1 = new PointPairList(inputs,outputs);
            PointPairList list2 = new PointPairList(x, y);
            LineItem myCurve;
            // Add the curves
            myCurve = myPane.AddCurve("points", list1, Color.Blue, SymbolType.Circle);
            myCurve.Line.IsVisible = false;
            myCurve.Symbol.Fill = new Fill(Color.Blue);

            myCurve = myPane.AddCurve("Simple", list2, Color.Red, SymbolType.Circle);
            myCurve.Line.IsAntiAlias = true;
            myCurve.Line.IsVisible = true;
            myCurve.Symbol.IsVisible = false;

            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Invalidate();
        }
    }
}
