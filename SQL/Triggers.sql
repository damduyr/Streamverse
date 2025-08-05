Create Trigger UpdateMovieRating
On Review
After Insert,Update,Delete
As
Begin

Declare @AffectedMovieIDs TABLE (MovieID int);

Insert into @AffectedMovieIDs (MovieID)
Select Distinct ContentID From inserted UNION 
Select distinct ContentID From deleted;

Update MOVIE
SET
OverallRating = (
Select AVG(CAST(r.RatingValue AS decimal(3,2))) 
From REVIEW r 
where r.ContentID = a.MovieID
)
From MOVIE Inner join @AffectedMovieIDs a 
ON MOVIE.MovieID = a.MovieID;
END
GO

