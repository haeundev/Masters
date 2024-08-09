## Haeun's phonetic training data (July 15 2024)
path <- "/Users/forclass/Source/Masters/VR Karting/Results"
in_wd <- paste(path, sep = "")
setwd(in_wd)
#library(dplyr)
library(tidyverse)
library(lme4)
library(lmerTest) # significance testing
library(ggplot2)
library(openxlsx) # seems to open large excel files with no problem
#library(stringr)

#files=dir(path=path,pattern=".xlsx")

# pre/mid/post tests ------------------------------------------------------
## file list
path %>%
  list.files() %>%
  .[str_detect(., ".xlsx")] -> file_names

## PRE
file_names %>%
  purrr::map(function(file_name){ # iterate through each file name
    read.xlsx(paste0(file_name),  sheet = "PRE", startRow = 1, colNames = FALSE)}) -> df_list_read

for (i in 1:length(df_list_read)) {
  df <- df_list_read[[i]]
  df$subj <- substring(file_names[i], 1, nchar(file_names[i])-5)
    if (i == 1) {
      df_all <- df[FALSE,]
    }
    df_all <- rbind(df_all, df)
    rm(df)
}

df_all$test <- "pre"

## MID
file_names %>%
  purrr::map(function(file_name){ # iterate through each file name
    read.xlsx(paste0(file_name),  sheet = "MID", startRow = 1, colNames = FALSE)}) -> df_list_read_m

for (i in 1:length(df_list_read_m)) {
  df <- df_list_read_m[[i]]
  df$subj <- substring(file_names[i], 1, nchar(file_names[i])-5)
  if (i == 1) {
    df_all_m <- df[FALSE,]
  }
  df_all_m <- rbind(df_all_m, df)
  rm(df)
}

df_all_m$test <- "mid"

## POST
file_names %>%
  purrr::map(function(file_name){ # iterate through each file name
    read.xlsx(paste0(file_name),  sheet = "POST", startRow = 1, colNames = FALSE)}) -> df_list_read_p

for (i in 1:length(df_list_read_m)) {
  df <- df_list_read_p[[i]]
  df$subj <- substring(file_names[i], 1, nchar(file_names[i])-5)
  if (i == 1) {
    df_all_p <- df[FALSE,]
  }
  df_all_p <- rbind(df_all_p, df)
  rm(df)
}

df_all_p$test <- "post"

df_all <- rbind(df_all, df_all_m, df_all_p)

df_all<- rename(df_all, word = X1, response = X2, cond = X3, speaker = X4)
df_all$vowel[df_all$word == "heed"] <- "i"
df_all$vowel[df_all$word == "hid"] <- "ɪ"
df_all$vpair[df_all$word == "heed"] <- 1
df_all$vpair[df_all$word == "hid"] <- 1

df_all$vowel[df_all$word == "had"] <- "æ"
df_all$vowel[df_all$word == "head"] <- "ɛ"
df_all$vpair[df_all$word == "had"] <- 2
df_all$vpair[df_all$word == "head"] <- 2

df_all$vowel[df_all$word == "hod"] <- "a"
df_all$vowel[df_all$word == "hud"] <- "ʌ"
df_all$vpair[df_all$word == "hod"] <- 3
df_all$vpair[df_all$word == "hud"] <- 3

df_all$vowel[df_all$word == "hood"] <- "ʊ"
df_all$vowel[df_all$word == "who'd"] <- "u"
df_all$vpair[df_all$word == "hood"] <- 4
df_all$vpair[df_all$word == "who'd"] <- 4

df_all$vowel[df_all$word == "bought"] <- "ɔ"
df_all$vowel[df_all$word == "boat"] <- "oʊ"
df_all$vpair[df_all$word == "bought"] <- 5
df_all$vpair[df_all$word == "boat"] <- 5

df_all$vowel[df_all$word == "bit"] <- "ɪ"
df_all$vowel[df_all$word == "bait"] <- "eɪ"
df_all$vpair[df_all$word == "bit"] <- 6
df_all$vpair[df_all$word == "bait"] <- 6

df_all$word <- as.factor(df_all$word)
df_all$vpair <- as.factor(df_all$vpair)
df_all$response <- as.factor(df_all$response)
df_all$cond <- as.factor(df_all$cond)
df_all$speaker <- as.factor(df_all$speaker)
df_all$subj <- as.factor(df_all$subj)
df_all$test <- as.factor(df_all$test)
df_all$vowel <- as.factor(df_all$vowel)

## Participant info (training conditions)
training <- read.xlsx('../ExperimentData.xlsx',  sheet = "Experiment", startRow = 2, colNames = FALSE)
training$group[training$X3 == "TRUE"] <- "control" 
training$group[training$X3 == "FALSE"] <- "noise and dual task"
training$group[training$X3 == "FALSE" & training$X12 == "None"] <- "dual task"

#### find the subject's group
sublist <- unique(df_all$subj)
df_all$group <- NA
for (i in 1:length(sublist)){
  if (sublist[i] == "514") {
    df_all$group[df_all$subj == "514"] <- "noise and dual task"
  } else if (sublist[i] == "3097") {
    df_all$group[df_all$subj == "3097"] <- "noise and dual task"
  } else if (sublist[i] == "6170") {
    df_all$group[df_all$subj == "6170"] <- "control"
  } else {
     df_all$group[df_all$subj == sublist[i]] <- training$group[which(sublist[i] == training$X2)]
  }
}

df_all$group <- as.factor(df_all$group)

### logistic mixed effects model 
mod1 <- glmer(response~ (1|subj)+ (1|word), data=df_all, family = "binomial")
mod2 <- update(mod1,.~.+ test)
anova(mod1, mod2)
mod3 <- update(mod1,.~.+ cond)
anova(mod1, mod3)
mod4 <- update(mod1,.~.+ vpair) # significant
anova(mod1, mod4)
mod5 <- update(mod4,.~.+ vpair:test) # significant
anova(mod4, mod5)
mod6 <- update(mod5,.~.+ vpair:cond) # not significant
anova(mod5, mod6)
mod7 <- update(mod5,.~.+ test:cond) # not significant
anova(mod5, mod7)
mod8 <- update(mod5,.~.+ vpair:test:cond) # not significant
anova(mod5, mod8)
mod9 <- update(mod5,.~.+ group) # not significant
anova(mod5, mod9)
mod10 <- update(mod5,.~.+ group:test) # not significant
anova(mod5, mod10)
mod11 <- update(mod5,.~.+ group:test:cond) # not significant
anova(mod5, mod11)
mod12 <- update(mod5,.~.+ group:test:vpair) # significant
anova(mod5, mod12)
mod13 <- update(mod12,.~.+ group:test:vpair:cond) # significant
anova(mod12, mod13)

## plotting
# avg_df <- df_all %>% 
#   count(response, subj, vpair, cond, test) %>%
#   group_by(subj, vpair, cond, test) %>%
#   mutate(freq = n / sum(n)) %>%
#   filter(response == "TRUE")
                     
avg_df <- df_all %>% 
  count(response, subj, vpair, test) %>%
  group_by(subj, vpair, test) %>%
  mutate(freq = n / sum(n)) %>%
  filter(response == "TRUE")

#only vowel pair 1 and 3 (just to explore)
avg_df2 <- df_all %>% filter(vpair == 1|vpair == 3) %>%
  count(response, subj, test, group) %>%
  group_by(subj, test, group) %>%
  mutate(freq = n / sum(n)) %>%
  filter(response == "TRUE")

#only vowel pair 1 and 3  (just to explore)å & depending on test type
avg_df3 <- df_all %>% filter(vpair == 1|vpair == 3) %>%
  count(response, subj, test, group, cond) %>%
  group_by(subj, test, group, cond) %>%
  mutate(freq = n / sum(n)) %>%
  filter(response == "TRUE")
# myplot1 <- avg_df %>%
#           mutate(test = fct_relevel(test, "pre", "mid")) %>%
#   ggplot(aes(x=cond, y=freq, fill = test)) + 
#   facet_grid(cols = vars(vpair)) +
#   scale_y_continuous(name = "accuracy", limits=c(0,1)) +
#   geom_violin() +
#   geom_boxplot(width=0.1, position = position_dodge(width = 0.9)) +
#   scale_fill_brewer(palette="BuPu")

# vowel pair across conditions
myplot1 <- avg_df %>%
  mutate(test = fct_relevel(test, "pre", "mid", "post")) %>%
  ggplot(aes(x=test, y=freq)) + 
  facet_grid(cols = vars(vpair)) +
  scale_y_continuous(name = "accuracy", limits=c(0,1)) +
  geom_violin() +
  geom_boxplot(width=0.1, position = position_dodge(width = 0.9)) +
  scale_fill_brewer(palette="BuPu")

# by group
myplot1 <- avg_df2 %>%
  mutate(test = fct_relevel(test, "pre", "mid", "post")) %>%
  ggplot(aes(x=group, y=freq, fill = test)) + 
  scale_y_continuous(name = "accuracy", limits=c(0,1)) +
  geom_violin() +
  geom_boxplot(width=0.1, position = position_dodge(width = 0.9)) +
  scale_fill_brewer(palette="BuPu")

# by group & test condition
myplot1 <- avg_df3 %>%
  mutate(test = fct_relevel(test, "pre", "mid", "post")) %>%
  ggplot(aes(x=group, y=freq, fill = test)) + 
  facet_grid(cols = vars(cond)) + 
  scale_y_continuous(name = "accuracy", limits=c(0,1)) +
  geom_violin() +
  geom_boxplot(width=0.1, position = position_dodge(width = 0.9)) +
  scale_fill_brewer(palette="BuPu")


myplot1 + theme_bw() +
  theme(axis.title.x = element_text(face = "bold", size=18), 
        axis.text.x = element_text(size=10),
        axis.title.y = element_text(face = "bold", size=18),
        axis.text.y = element_text(size=18),
        strip.text.x = element_text(size = 18, face = "bold"),
        strip.text.y = element_text(size=18, face="bold"),
        plot.title = element_text(size=19, face="bold", vjust=2, hjust = 0.5),
        panel.grid.major = element_blank(), 
        panel.grid.minor = element_blank(),
        #legend.position="none",
        legend.title = element_text(size=17),
        legend.text = element_text(size=17),
        axis.line = element_line(colour = "black"))

ggsave("premidpost_prelim.png", width = 12, height = 4)

ggsave("premidpost_v13_group.png", width = 10, height = 5)
ggsave("premidpost_v13_group_cond.png", width = 12, height = 4)