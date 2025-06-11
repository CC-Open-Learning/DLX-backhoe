/*
 *  FILE          : IInspectable.cs
 *  PROGRAMMER    :	Leon Vong
 *  FIRST VERSION :	2021-19-08
 *  DESCRIPTION   : This file contains the IInspectable<T> interface.
 *                  This is a very basic interface that is used to check
 *                  if said functionality is complete with a generic object
 *                  to compare to.
 */

namespace RemoteEducation.Interactions
{
    public interface IInspectable<T>
    {
        void CheckIfComplete(T comparisonObject);
    }
}