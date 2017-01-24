require(ggplot2)
require(reshape2)

readFile <- function(fileName) {
  
  all_content = readLines(paste("test5/", fileName, sep = ""))
  #skip_first = all_content[-1]
  dat = read.csv(textConnection(all_content), header = TRUE, stringsAsFactors = FALSE, sep = ';')
  return(dat)
}

combineData <- function(fileNames, resultNames, columnName, maxValue) {
  
  result <- data.frame(step= numeric(), series= factor(), value = numeric())
  columnIndex <- 1
  i <- 1
  
  for(fileName in fileNames){
    dat = readFile(fileName)
    
    for(n in 1:nrow(dat)) {
      de<-data.frame(dat[n, 'step'],resultNames[columnIndex], dat[n, columnName])
      names(de)<-c("step","series", "value")
      result <- rbind(result, de)
      i <- i + 1
      
      if(dat[n, columnName] == maxValue) {
        break
      }
    }
    columnIndex <- columnIndex + 1
  }
  
  return(result)
}

maxValue <- 168
header <- c("Brak dzielenia wiedzy",
            "Kara za dzielenie: 0", 
            "Kara za dzielenie: 100", 
            "Kara za dzielenie: 1000",
            "Kara za dzielenie: 10000")
df = combineData(c("data_nosharing.txt", "data_penalty_0.txt", "data_penalty_100.txt", "data_penalty_1000.txt", "data_penalty_10000.txt"), header, "danger", 9999999)

ggplot(df, aes(step,value)) +
   geom_line(aes(colour = series)) +
   xlab("Liczba kroków symulacji") +
   ylab("Iloœæ napotkanych zagro¿eñ") +
   #geom_hline(yintercept=maxValue, linetype = 2,show.legend =TRUE) +
   # annotate("text",x=500,y=maxValue + 10,size=3,label=c('Ca³kowita iloœæ zagro¿eñ')) +
   theme(legend.title=element_blank(), legend.position="bottom")

# or plot on different plots
#ggplot(df, aes(step,value)) + geom_line() + facet_grid(series ~ .)
