/*
 *  FILE          :	TableWrapper.cs
 *  PROJECT       :	CORE (XML Config)
 *  PROGRAMMER    :	Kieron Higgs
 *  FIRST VERSION :	2020-07-17
 *  DESCRIPTION   : 
 */

#region Resources

using System;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace RemoteEducation.Scenarios
{
    /*
     * CLASS      : TableWrapper
     * DESCRIPTION: 
     */
    [Serializable]
    [XmlType("table")]
    [XmlRoot("table")]
    public class TableWrapper
    {
        #region Fields

        public string[] columns;
        public string[] rows;

        #endregion

        #region Public methods

        /*
         * CONSTRUCTOR : TableWrapper()
         * DESCRIPTION :
         *      Instantiates a TableWrapper for import/export of interactable objects
         * PARAMETERS  :
         *      Transform tableT -
         */
        public TableWrapper(TableCanvas tableCanvas)
        {
            rows = tableCanvas.rowLabels;
            columns = tableCanvas.columnLabels;
        }

        /*
        * CONSTRUCTOR : TableWrapper()
        * DESCRIPTION :
        *      An empty constructor for the TableWrapper class (required for XML-style serialization).
        */
        public TableWrapper()
        {

        }

        /*
         * METHOD    : CopyWrapperData()
         * DESCRIPTION : 
         *      
         */
        public void CopyWrapperData(TableCanvas tableCanvas)
        {
            tableCanvas.rowLabels = rows;
            tableCanvas.columnLabels = columns;
        }

        #endregion
    }
}