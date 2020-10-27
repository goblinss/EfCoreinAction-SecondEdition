﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.DefaultSql.Books
{
    public class SortFilterPageOptions
    {
        public const int DefaultPageSize = 100; //default page

        //-----------------------------------------
        //Paging parts, which require the use of the method

        private int _pageNum = 1;

        private int _pageSize = DefaultPageSize;

        /// <summary>
        ///     This holds the possible page sizes
        /// </summary>
        public int[] PageSizes = new[] {5, 10, 20, 50, 100, 500, 1000};

        public OrderByOptions OrderByOptions { get; set; }

        public BooksFilterBy FilterBy { get; set; }

        public string FilterValue { get; set; }

        public int PageNum
        {
            get { return _pageNum; }
            set { _pageNum = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }


        /// <summary>
        ///     This is set to the number of pages available based on the number of entries in the query
        /// </summary>
        public int NumPages { get; private set; }

        /// <summary>
        ///     This holds the state of the key parts of the SortFilterPage parts
        /// </summary>
        public string PrevCheckState { get; set; }


        public async Task SetupRestOfDtoAsync<T>(IQueryable<T> query)
        {
            SetupRestOfDto(await query.CountAsync());
        }

        public void SetupRestOfDto(int rowCount)
        {
            NumPages = (int)Math.Ceiling(
                ((double)rowCount) / PageSize);
            PageNum = Math.Min(
                Math.Max(1, PageNum), NumPages);

            var newCheckState = GenerateCheckState();
            if (PrevCheckState != newCheckState)
                PageNum = 1;

            PrevCheckState = newCheckState;
        }

        //----------------------------------------
        //private methods

        /// <summary>
        ///     This returns a string containing the state of the SortFilterPage data
        ///     that, if they change, should cause the PageNum to be set back to 0
        /// </summary>
        /// <returns></returns>
        private string GenerateCheckState()
        {
            return $"{(int) FilterBy},{FilterValue},{PageSize},{NumPages}";
        }
    }
}