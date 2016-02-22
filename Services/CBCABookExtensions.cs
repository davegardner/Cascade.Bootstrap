using System;
using System.Collections.Generic;
using System.Linq;

namespace Cascade.Bootstrap.Services
{
    public static class CBCABookExtensions
    {
        class Contributor
        {
            public string Name;
            public string Role;
            public int Sequence;
        }

        public static string GetCredits(dynamic book)
        {
            string credits = String.Empty;
            bool isPictureBook = book.BookAwardPart.Category.Value.IndexOf("picture book", StringComparison.OrdinalIgnoreCase) == 0;

            List<Contributor> contributors = new List<Contributor>();
            List<Contributor> groupedContributors = new List<Contributor>();

            contributors.Add(new Contributor
            {
                Name = (book.BookPart.BookPart.Author1FirstName.Value + " " + book.BookPart.Author1LastName.Value).Trim(),
                Role = book.BookPart.Author1Role.Value,
                Sequence = 1
            });
            contributors.Add(new Contributor
            {
                Name = (book.BookPart.Author2FirstName.Value + " " + book.BookPart.Author2LastName.Value).Trim(),
                Role = book.BookPart.Author2Role.Value,
                Sequence = 2
            });
            contributors.Add(new Contributor
            {
                Name = (book.BookPart.Author3FirstName.Value + " " + book.BookPart.Author3LastName.Value).Trim(),
                Role = book.BookPart.Author3Role.Value,
                Sequence = 3
            });
            contributors.Add(new Contributor
            {
                Name = (book.BookPart.Author4FirstName.Value + " " + book.BookPart.Author4LastName.Value).Trim(),
                Role = book.BookPart.Author4Role.Value,
                Sequence = 4
            });


            // first, concatenate similar roles (eg if two 'text' roles then delete one and make the other 'James Nobody and Julie Burke')
            var groups = contributors.Where(b=>!string.IsNullOrEmpty(b.Name)).GroupBy(b => b.Role);
            foreach (var group in groups)
            {
                groupedContributors.Add(new Contributor
                {
                    Name = string.Join(" & ", group.Select(g => g.Name).ToArray()),
                    Role = group.Key,
                    Sequence = group.Min(g => g.Sequence)
                });
            }

            // Adjust sequence for Picture Books by placing illustrator first
            if (isPictureBook)
            {
                foreach( var contributor in groupedContributors.Where(g => g.Role.StartsWith("illust", StringComparison.OrdinalIgnoreCase)))
                {
                    contributor.Sequence = 0;
                }
                groupedContributors = groupedContributors.OrderBy(b => b.Sequence).ToList();
            }

            // Assemble non-empty credits into a string like "Jo Blow (illust: Fred Nurk & Julie Bishop, ed: Per Diem, photos: Murray Ware)"
            for (var index = 0; index < groupedContributors.Count; index++)
            {
                if (index == 0)
                {
                    // first credit
                    credits += groupedContributors[index].Name;
                }
                else
                {
                    if (index == 1)
                    {
                        // second credit
                        credits += " (" + groupedContributors[index].Role + ": " + groupedContributors[index].Name;
                    }
                    else
                    {
                        // third and fourth credits
                        credits += ", " + groupedContributors[index].Role + ": " + groupedContributors[index].Name;
                    }
                    credits += ")";
                }
            }
            return credits;
        }
    }
}
