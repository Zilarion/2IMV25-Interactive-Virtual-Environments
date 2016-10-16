#------------- Init -----------------
require(plyr)

# Load in data
filenames <- list.files("data", pattern="*.csv", full.names=TRUE)
ldf <- lapply(filenames, read.csv)

#------------- JND over time -----------------
# Only get k of each batch
res <- sapply(ldf, function(x) x["k"][seq(1, nrow(x), 9),])
maxL <- max(sapply(res, length))


# Fill up last k until maximum amount of batches in data
res <- plyr::ldply(res, rbind)
for (i in 1:dim(res)[1]) {
	index <- max(which(!is.na(res[i,])))
	row <- res[i,]
	row[is.na(row)] <- res[i,index]
	res[i,] <- row
}

# Make list out of the matrix
res <- as.data.frame(res)

# Boxplot of k over time 
boxplot(x=res, main="JND over time", xlab="Batch number", ylab="k")


#------------- errors over time -----------------
# Plot correct false over batch time
correctness <- sapply(ldf, function(x) x["result"][,])


# Create list of vectors with percentage of correct answers in each batch
correctness_data <- list()
for (i in 1:length(correctness)) {
	test <- correctness[[i]]
	cl <- vector()
	for (j in seq(from=1, to=length(test), by=9)) {
		var <- test[j:(j+8)]
		batchnr <- (j-1)/9+1
		cl <- c(cl, sum(var=="False")/9)
	}
	correctness_data[[i]] <- cl
}
correctness_data <- plyr::ldply(correctness_data, rbind)
correctness_data <- lapply(correctness_data, function(x) x[!is.na(x)])

boxplot(x=correctness_data, main="Errors over time", xlab="Batch number", ylab="Number correct")