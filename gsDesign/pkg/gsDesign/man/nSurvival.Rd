\name{nSurvival}
\alias{nSurvival}
\alias{nEvents}
\alias{print.nSurvival}
\alias{Survival sample size}
\title{3.4: Time-to-event sample size calculation (Lachin-Foulkes)}
\description{\code{nSurvival()} is used to calculate the sample size for a clinical trial with a time-to-event endpoint. The Lachin and Foulkes (1986) method is used.
}

\usage{
nSurvival(lambda1=1/12, lambda2=1/24, Ts=24, Tr=12, eta = 0, ratio = 1,
      alpha = 0.025, beta = 0.10, sided = 1, approx = FALSE,
      type = c("rr", "rd"), entry = c("unif", "expo"), gamma = NA)
print.nSurvival(x,...)
nEvents(hr = .6, alpha = .025, beta = .1, ratio = 1, sided = 1, hr0 =  1, n = 0, tbl = FALSE)
}
\arguments{
  \item{lambda1, lambda2}{event hazard rate for placebo and treatment
    group respectively.}
  \item{eta}{equal dropout hazard rate for both groups.}
  \item{ratio}{randomization ratio between placebo and treatment
    group. Default is balanced design, i.e., randomization ratio is 1.}
  \item{Ts}{maximum study duration.}
  \item{Tr}{accrual (recruitment) duration.}
  \item{alpha}{type I error rate. Default is 0.05 since 2-sided testing is default.}
  \item{beta}{type II error rate. Default is 0.10 (90\% power). Not needed for \code{nEvents()} if n is provided.}
  \item{sided}{one or two-sided test? Default is two-sided test.}
  \item{approx}{logical. If \code{TRUE}, the approximation sample size
    formula for risk difference is used.}
  \item{type}{type of sample size calculation: risk ratio (\dQuote{rr}) or risk
    difference (\dQuote{rd}).}
  \item{entry}{patient entry type: uniform entry (\code{"unif"}) or exponential
    entry (\code{"expo"}).}
  \item{gamma}{rate parameter for exponential entry. \code{NA} if entry type is
    \code{"unif"} (uniform). A non-zero value is supplied if entry type is
    \code{"expo"} (exponential).} 
  \item{x}{An object of class "nSurvival" returned by \code{nSurvival()}
   (optional: used for output; "months" or "years" would be the 'usual' choices).}
  \item{hr}{Hazard ratio under the alternative hypothesis (>0).}
  \item{hr0}{Hazard ratio under the null hypothesis (>0, != \code{hr}).}
  \item{n}{Number of events may be input to compute power rather than sample size.}
  \item{tbl}{Indicator of whether or not scalar (vector) or tabular output is desired for \code{nEvents()}.}
  \item{...}{Allows additional arguments for \code{print.nSurvival()}.}
}

\details{
  \code{nSurvival()} produces an object of class "nSurvival" with 
  the number of subjects and events for a set of
  pre-specified trial parameters, such as accrual duration and follow-up
  period. The calculation is based on Lachin and Foulkes (1986) method and can
  be used for risk ratio or risk difference. The function also consider
  non-uniform (exponential) entry as well as uniform entry. 

  If the logical approx is \code{TRUE}, the variance under alternative
  hypothesis is used to replace the variance under null hypothesis.
  For non-uniform entry, a non-zero value of gamma for exponential entry
  must be supplied. For positive gamma, the entry distribution is
  convex, whereas for negative gamma, the entry distribution is concave.

  \code{nEvents()} uses the Schoenfeld (1981) method to approximate the number of events \code{n} (given \code{beta}) or the power (given \code{n}). Arguments may be vectors or scalars, but any vectors must have the same length.
}

\value{
  \code{nSurvival} produces a list with the following component returned:
  \item{type}{As input.}
  \item{entry}{As input.}
  \item{n}{Sample size required (computed).}
  \item{nEvents}{Number of events required (computed).}
  \item{lambda1}{As input.}
  \item{lambda2}{As input.}
  \item{eta}{As input.}
  \item{ratio}{As input.}
  \item{gamma}{As input.}
  \item{alpha}{As input.}
  \item{beta}{As input.}
  \item{sided}{As input.}
  \item{Ts}{As input.}
  \item{Tr}{As input.}

  \code{nEvents} produces a scalar or vector of sample sizes (or powers) when \code{tbl=FALSE} or, when \code{tbl=TRUE} a matrix of values with the following columns:
  \item{hr}{As input.}
  \item{n}{If \code{n[1]=0} on input (default), output contains the number of events need to obtain the input Type I and II error. If \code{n[1]>0} on input, the input value is returned.}
  \item{alpha}{As input.}
  \item{beta}{If \code{n[1]=0} on input (default), \code{beta} is output as input. Otherwise, this is the computed Type II error based on the input \code{n}.}
  \item{Power}{One minus the output \code{beta}. When \code{tbl=FALSE, n[1]>0}, this is the value or vector of values returned.}
  \item{delta}{Standardized effect size represented by input difference between null and alternative hypothesis hazard ratios.}
  \item{ratio}{Ratio of experimental to control sample size where 'experimental' is the same as the group with hazard represented in the numerator of the hazard ratio.}
  \item{se}{Estimated standard error for the observed log(hazard ratio) with the given sample size.}
}
\seealso{\link{gsDesign package overview}, \link{Plots for group sequential designs}, \link{gsDesign}} 
\author{Shanhong Guan \email{shanhong\_guan@merck.com}}
\references{
  Lachin JM and Foulkes MA (1986),
  Evaluation of Sample Size and Power for Analyses of Survival
  with Allowance for Nonuniform Patient Entry, Losses to Follow-Up,
  Noncompliance, and Stratification. \emph{Biometrics}, 42, 507-519.

  Schoenfeld D (1981),
  The Asymptotic Properties of Nonparametric Tests for Comparing Survival Distributions. \emph{Biometrika}, 
  68, 316-319. 
}

\examples{

# consider a trial with 
# 2 year maximum follow-up
# 6 month uniform enrollment
# Treatment/placebo hazards = 0.1/0.2 per 1 person-year
# drop out hazard 0.1 per 1 person-year
# alpha = 0.025 (1-sided)
# power = 0.9 (default beta=.1)

ss <- nSurvival(lambda1=.2 , lambda2=.1, eta = .1, Ts = 2, Tr = .5,
                sided=1, alpha=.025)
ss

#  symmetric, 2-sided design with O'Brien-Fleming-like boundaries
#  sample size is computed based on a fixed design requiring n=100
	x<-gsDesign(k = 5, test.type = 2, n.fix=ss$nEvents, nFixSurv=ss$n)
# boundary plot
	plot(x)
# effect size plot
	plot(x, plottype = "hr")
# total sample size
   x$nSurv
# number of events at analyses
   x$n.I
# print the design
x

# approximate number of events required using Schoenfeld's method
# for 2 different hazard ratios
nEvents(hr=c(.5, .6), tbl=TRUE)
# vector output
nEvents(hr=c(.5, .6))

# approximate power using Schoenfeld's method
# given 2 sample sizes and hr=.6
nEvents(hr=.6, n=c(50, 100), tbl=TRUE)
# vector output
nEvents(hr=.6, n=c(50, 100))
}

\keyword{design}
