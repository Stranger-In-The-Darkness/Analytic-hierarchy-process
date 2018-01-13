using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetMatrix;

namespace MAI
{
    public class AHPmodel
    {
        //number of criteria
        private int _nCriteria;
        //number of choices
        private int _mChoices;
        //criteria times choices
        private int _superDim;

        //scored criteria
        private GeneralMatrix _criteria;

        //choices scored for all criteria such that
        //if there are n n criteria and m choices
        //the choice matrix will have a dimention of n by n*m
        private GeneralMatrix _choiceMatrix;

        //calculated criteria
        private GeneralMatrix _orderedCriteria;

        //calculated choices
        private GeneralMatrix _calculatedChoices;

        //final result of the model - matrix of m (choices) by 1
        //calculated as a product of criteria selection times
        //choice selection for each criteria = choices weighted 
        //by criteria
        private GeneralMatrix _modelResult;

        //consistency ratio matrix for each comparison. Each should be equal or
        //less to 10% for consistent model. First row represents
        //Consistency ratio for the criteria of the model
        private GeneralMatrix _lambdas;


        /// <summary>
        /// Parametrized Constructor
        /// </summary>
        /// <param name="n">number of selection criteria in the model</param>
        /// <param name="m">number of choices in the model</param>
        public AHPmodel(int n, int m)
        {
            if ((n > 20) || (m > 20))
                throw new ArgumentException("models with over 20 criteria and /or choices are not supported");
            _superDim = n * m;
            _nCriteria = n;
            _mChoices = m;
            _criteria = new GeneralMatrix(n, n);
            _choiceMatrix = new GeneralMatrix(m, _superDim);
            _orderedCriteria = new GeneralMatrix(n, 1);
            _modelResult = new GeneralMatrix(m, n);
            _calculatedChoices = new GeneralMatrix(m, 1);
            _lambdas = new GeneralMatrix(n + 1, 1);
        }

        #region Accessors

        /// <summary>
        /// Criteria priorities as calculated by the model.
        /// A matrix of n by 1
        /// </summary>
        public GeneralMatrix CalculatedCriteria
        {
            get { return _orderedCriteria; }

        }

        /// <summary>
        /// Raw criteria scored after the pairwise comparison
        /// A matrix of n by n
        /// </summary>
        public GeneralMatrix RatedCriteria
        {
            get { return _criteria; }
            set
            {
                _criteria = ExpandUtility(value);
                _criteria = value;
            }
        }

        /// <summary>
        /// Model choices scored in pairwise comparison for each criteria
        /// A matrix of n by n*m
        /// </summary>
        public GeneralMatrix ChoiceMatrix
        {
            get { return _choiceMatrix; }
        }

        /// <summary>
        /// Result
        /// </summary>
        public GeneralMatrix ModelResult
        {
            get { return _modelResult; }
        }

        /// <summary>
        /// 
        /// </summary>
        public GeneralMatrix CalculatedChoices
        {
            get { return _calculatedChoices; }
        }

        /// <summary>
        /// Matrix of consistency ratios for the model
        /// size of n+1 where n is number of criteria
        /// First row represents consistency ratio for the 
        /// 
        /// </summary>
        public GeneralMatrix ConsistencyRatio
        {
            get { return _lambdas; }
        }

        #endregion

        #region Public Functions

        public void AddCriteria(GeneralMatrix matrix)
        {
            _criteria = ExpandUtility(matrix);
        }

        public void AddCriteria(double[][] matrix)
        {
            GeneralMatrix newMatrix = new GeneralMatrix(matrix);
            AddCriteria(newMatrix);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="criterionId"></param>
        public void AddCriterionRatedChoices(int criterionId, GeneralMatrix matrix)
        {
            if (criterionId > _nCriteria)
                throw new ArgumentException("Passed criterion ID greater than number of criteria");


            int col0 = criterionId * _mChoices;
            int colMax = col0 + _mChoices - 1;
            GeneralMatrix newMatrix = (GeneralMatrix)ExpandUtility(matrix).Clone();
            _choiceMatrix.SetMatrix(0, _mChoices - 1, col0, colMax, newMatrix);
        }

        public void AddCriterionRatedChoices(int criterionId, double[][] matrix)
        {
            GeneralMatrix gMatrix = new GeneralMatrix(matrix);
            AddCriterionRatedChoices(criterionId, gMatrix);
        }

        /// <summary>
        /// (
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static GeneralMatrix ExpandUtility(GeneralMatrix matrix)
        {
            double val = 0.0;
            int n = matrix.RowDimension;
            int m = matrix.ColumnDimension;

            if (n != m)
                throw new ArgumentException("Criteria matrix must be symmetrical");

            GeneralMatrix newMatrix = matrix.Transpose();

            //for all transposed elements calculate their inverse values
            //set diagonal elements to 0
            for (int i = 0; i < n; i++)
                for (int j = 0; j <= i; j++)
                {
                    val = newMatrix.GetElement(i, j);
                    if (val == 0.0)
                        throw new ArgumentException("Criteria comparison values won't be 0");
                    newMatrix.SetElement(i, j, 1 / val);
                    if (i == j)
                        newMatrix.SetElement(i, j, 0);
                }
            //add transposed, inverse matrix to the original one
            //create fully expanded matrix 
            return newMatrix.Add(matrix);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CalculateModel()
        {
            CalculatePriorities();
            CalculateChoices();
            CalculateFinalResult();
        }

        #endregion

        #region Private functions
        /// <summary>
        /// 
        /// </summary>
        private void CalculatePriorities()
        {
            PrioritiesSelector selector = new PrioritiesSelector();
            selector.ComputePriorities(_criteria);

            //first (zero-th) element of the lambda matrix is the
            //consistency ratio factor for the selection matrix

            _lambdas.SetElement(0, 0, selector.ConsistencyRatio);

            _orderedCriteria = selector.CalculatedMatrix;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateChoices()
        {
            GeneralMatrix tempMatrix = null;
            int i, col0, colMax;

            PrioritiesSelector selector = new PrioritiesSelector();

            for (i = 0; i < this._nCriteria; i++)
            {
                col0 = i * _mChoices;
                colMax = col0 + _mChoices - 1;
                tempMatrix = _choiceMatrix.GetMatrix(0, _mChoices - 1, col0, colMax);
                selector.ComputePriorities(tempMatrix);
                //first element of the matrix is consistency ratio
                //for the criteria

                _lambdas.SetElement(i + 1, 0, selector.ConsistencyRatio);

                _modelResult.SetMatrix(0, _mChoices - 1, i, i, selector.CalculatedMatrix);

            }

        }

        /// <summary>
        /// Calculates final model results as a sum of product of choices rated
        /// for each criteria times weighing (preference) of each criteria
        /// </summary>
        private void CalculateFinalResult()
        {
            double sum;
            for (int i = 0; i < _mChoices; i++)
            {
                sum = 0;
                for (int j = 0; j < _nCriteria; j++)
                {
                    sum += _modelResult.GetElement(i, j) * this._orderedCriteria.GetElement(j, 0);
                }
                _calculatedChoices.SetElement(i, 0, sum);
            }

        }

        #endregion

    }

}
    
