/*
 *  FILE          : IInitializable.cs
 *  PROJECT       : Core Engine
 *  PROGRAMMER    :	Duane Cressman
 *  FIRST VERSION :	2020-12-02
 *  DESCRIPTION   : This file contains the IInitializable interface.
 *                  This is a very basic interface that is used to
 *                  implement an Initialize method.
 */

namespace RemoteEducation.Interactions
{
    public interface IInitializable
    {
        void Initialize(object input = null);
    }
}