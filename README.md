# **Root-Causing and Event Identification Through Sensor Data**

The readme contains work which is not in the final document but is used to show how and what methodologies used in this project. It will also contain documentation on how to use the program which will be developed along side the project.

This will be updated as the project progress'

Relational Event Prediction System (REPS)

I present REPS, a graph-based real-time analysis tool which developers themselves can design to fit their use case while making use of many state-of-the-art machine learning models. REPS operates by processing input data, which is likely will be sensor data which will pass through nodes called "event nodes". Each node processes inputs from one or more sources (a sensor or event node) and generates a result that can be used on one or multiple downstream nodes. This system enables	real-time decision-making across interconnected components.

# Term & Synonym brainstorm

| Term | Synonyms |
|:----:|----------|
| Event | Act, Action, Case, Circumstance, Crisis, Development, Episode, Experience, Incident, Occasion, Situation, Advent, Calamity, Catastrophe, Concuncture, Emergency, Exploit, Occurrence, phenomenon |
| Prediction | Forecasting, Indicate, Prognostication, Anticipation |
| State | Case, Element, Position, Status |
| Change | Adjustment, development, revision, mutation, correction, transmutation |
| Value | Content |
| Rule | Decree, Guideline, Law, Regulation, Ruling, Statute, Criterion, Dictum, Guide, Model, Prescription, Precept, Policy |
| Trigger | Cause, Activater, Set off, Give rise to, elicit |

First search is for 'Event Prediction' on Scopus

First Search engine used is Scopus, following Query is used:
|TITLE-ABS-KEY ( ( event OR act OR action OR case OR circumstance OR crisis OR development OR episode OR experience OR incident OR occasion OR situation OR advent OR calamity OR catastrophe OR concuncture OR emergency OR exploit OR occurrence OR phenomenon ) AND ( prediction OR forecasting OR indicate OR prognostication OR anticipation ) ) AND PUBYEAR > 2014 AND PUBYEAR < 2026 AND PUBYEAR > 2014 AND PUBYEAR < 2024 AND ( LIMIT-TO ( DOCTYPE , "cp" ) OR LIMIT-TO ( DOCTYPE , "ar" ) ) AND ( LIMIT-TO ( SUBJAREA , "COMP" ) OR LIMIT-TO ( SUBJAREA , "MATH" ) ) AND ( LIMIT-TO ( LANGUAGE , "English" ) )|

Giving 172.968 results, as this is too much, it is limited down to highest 200 citations. Then sorted by what is deemed as relevant articles, and then sorted by relevant abstracts. Should have exscluded certain fields but what is gathered should suffice, otherwise a new search can be made. Giving 32 potiential articles.

Same process will be repeated for other scientific databases.

Forskningsportalen:

Query:
https://local.forskningsportal.dk/search/153837

( event OR act OR action OR case OR circumstance OR crisis OR development OR episode OR experience OR incident OR occasion OR situation OR advent OR calamity OR catastrophe OR concuncture OR emergency OR exploit OR occurrence OR phenomenon ) AND ( prediction OR forecasting OR indicate OR prognostication OR anticipation ) AND Publication Types=(Journal Article OR Conference Paper OR Thesis PhD) AND Publication Audiences=(Scientific) AND	Publication Status=(Published) AND	Review Status=(Peer Reviewed) AND Language=(English OR Danish) AND	Open Access=(Open Access) AND DK Main Research Areas=(Science/Technology) AND Years=(2024 OR 2023 OR 2022 OR 2021 OR 2020 OR 2019 OR 2018 OR 2017 OR 2016 OR 2015 OR 2014)

Giving 7.509 results, here after we sort by relevance and sorting through the 100 first. Resulting in 8 potiential articles.

From the 40 total articles, 24 have been selected based on their abstract.



## List of scenarios where the project idea seems usable

| Scenario | Reason |
| :------: | :----: |
| RUL (remaining usefull life) | The individual parts of a system is known and definable |
| Specific Code vulnerbility analysis | seems a stretch and have to be thought of a bit more |
| Trafic control | Limited variables to control for and all systems can the developer have in mind |
| Windmill | All components are known and often times recorded |
| Software Systems with multiple components/services | Limited variables to control for and all systems can the developer have in mind |

## List of scenarios where the project idea seems unsuable

Scenarios with high complexity with many variables to a degree which is undefineable.

| Scenario | Reason |
| :------: | :----: |
| Weather Forecasting | Too many varaibles, other models clearly has an advantage |

# Potiential features

- Modability during run-time
- Addition or removal of nodes
- Hybrid with other predictive models
- Special repeatable cases such as holidays, check whether machines are turned off.

## Possible Features

After reading, I've a curiosity has sprung. Is it possible using machine learning to auto generate the nodes in the model. Fewer articles will be read about this subject. The idea is to see whether it is a posibility to add this to the model, but it is not intended to be part of the main feature.

| Term | Synonyms |
|:----:|----------|
| AI | Machine learning, Deep learning, Auto generation, Self generation, Self constructing, Graph generation |
| Node Net Generation | Path Generation, Relation map Generation, Graph map Generation |

Using ACM DL using the following Query:
[[All: "node net generation"] OR [All: "path generation"] OR [All: "relation map generation"] OR [All: "graph map generation"]] AND [[All: "ai"] OR [All: "machine learning"] OR [All: "deep learning"] OR [All: "auto generation"] OR [All: "self generation"] OR [All: "self constructing"] OR [All: "graph generation"]]

Date : 2008-2024
Journal & Research Article

Giving 60 results in total sorted by most cited


# NOTES

Kort of de danske veje i en digital form, man kan derfra 
Hårde regler mod bløde regler
f.eks. man kan ikke være yngre end ens barn vs. meget høj internet trafik kan være tegn på DOS angreb


# Analysis Phase
This is the start of the software development process, I will descripe any requirements found, the technology needed to fulfill these requirements and a general design of the system based on these requirements and technology.

## Functional requirements

| Requirement ID | Requirement | Prioritization Level | Description | Reason | Comments |
| :------------: | :---------: | :------------------: | :---------: | :----: | :------: |
| FR0 | Having individual process in each 'Event Node' | Must have | An 'Event Node' is the main process of make predictions and conclusions | It is the point that these process' can be used to make predictions and conclusions | |
| FR1 | Is able to communicate with others services which is not part of the REPS | Must have | Other system must be able to get values from REPS to make determinations based on the states REPS notices | For other system can react based on the states REPS notices, it need to share it, otherwise there is no need for REPS | |
| FR2 | Being able to easily add new 'Event Nodes' | Could have | Using XML or JSON, one could easily add new nodes to the REPS System | It could speed up testing, but it is not a requirement for the system to function | This would require FR3 |
| FR3 | Being able to quickly spun up REPS using XML or JSON | Could have | Could allow for implementing the system quicker and allow for sharing REPS' | It could speed up testing, but it is not a requirement for the system to function | |
| FR4 | Allow for ML Models in the 'Event Nodes' | Should have | This could really expand the use cases for REPS as companies may have their own or develop one for their problem | It is very usefull for this project, but not required to be seen as succesfull | |