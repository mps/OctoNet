﻿using System;

namespace OctoNet.Models
{
    public static class Extensions
    {
        public static string GetText(this Filter filter)
        {
            switch (filter)
            {
                case Filter.Assigned:
                    return "assigned";
                case Filter.Created:
                    return "created";
                case Filter.Mentioned:
                    return "mentioned";
                case Filter.Subscribed:
                    return "subscribed";
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string GetText(this State state)
        {
            switch (state)
            {
                case State.Open:
                    return "open";
                case State.Closed:
                    return "closed";
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string GetText(this SortBy sort)
        {
            switch (sort)
            {
                case SortBy.Created:
                    return "created";
                case SortBy.Updated:
                    return "updated";
                case SortBy.Comments:
                    return "comments";
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string GetText(this OrderBy direction)
        {
            switch (direction)
            {
                case OrderBy.Ascending:
                    return "asc";
                case OrderBy.Descending:
                    return "desc";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}