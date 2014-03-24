using System;
using System.Collections;
using System.Collections.Generic;

namespace TidyNet
{
	/// <summary>
	/// Collection of TidyMessages
	/// </summary>
	[Serializable]
    public class TidyMessageCollection : List<TidyMessage>
	{
		/// <summary>
		/// Public default constructor
		/// </summary>
		public TidyMessageCollection()
		{
		}

        ///// <summary>
        ///// Adds a message.
        ///// </summary>
        ///// <param name="message">The message to add.</param>
        //public void Add(TidyMessage message)
        //{
        //    if (message.Level == MessageLevel.Error)
        //    {
        //        _errors++;
        //    }
        //    else if (message.Level == MessageLevel.Warning)
        //    {
        //        _warnings++;
        //    }

        //    InnerList.Add(message);
        //}

        /// <summary>
        /// Gets a value indicating whether needs author intervention.
        /// </summary>
        /// <value>
        /// <c>true</c> if needs author intervention; otherwise, <c>false</c>.
        /// </value>
        public virtual bool NeedsAuthorIntervention
        {
            get { return Errors > 0; }
        }

		/// <summary> Errors - the number of errors that occurred in the most
		/// recent parse operation
		/// </summary>
		public virtual int Errors
		{
			get
			{
                return ErrorMessages.Count;
            }
		}
		
		/// <summary> Warnings - the number of warnings that occurred in the most
		/// recent parse operation
		/// </summary>
		public virtual int Warnings
		{
			get
			{
                return WarningMessages.Count;
            }
		}

        public virtual ICollection<TidyMessage> ErrorMessages
        {
            get
            {
                return FindAll(message => message.Level == MessageLevel.Error);
            }
        }

        public virtual ICollection<TidyMessage> WarningMessages
        {
            get
            {
                return FindAll(message => message.Level == MessageLevel.Warning);
            }
        }
	}
}
