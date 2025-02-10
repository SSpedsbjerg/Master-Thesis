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

Any further documentation is in the excel sheet

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

# **Development Phase**
From this point on, it will be going into the development phase based on the background research done.

# Weekly Goals
Here, weekly goals will be written down to document the work.
| Week | Goals | Comments |
| :--: | :---: | :------: |
| 6 | Analysis of the system in forms of requirements and technology; modify or find a system which REPS can fully encompass | Unsure about the second goal, whether it is achieveable this week |


# Analysis Phase
This is the start of the software development process, I will descripe any requirements found, the technology needed to fulfill these requirements and a general design of the system based on these requirements and technology.

## Functional Requirements
I will be using MoSCoW analysis method as this is the one I see fiting best.

| Requirement ID | Requirement | Prioritization Level | Description | Reason | Comments |
| :------------: | :---------: | :------------------: | :---------: | :----: | :------: |
| FR0 | Having individual process in each 'Event Node' | Must have | An 'Event Node' is the main process of make predictions and conclusions | It is the point that these process' can be used to make predictions and conclusions | |
| FR1 | Is able to communicate with others services which is not part of the REPS | Must have | Other system must be able to get values from REPS to make determinations based on the states REPS notices | For other system can react based on the states REPS notices, it need to share it, otherwise there is no need for REPS | |
| FR2 | Being able to easily add new 'Event Nodes' | Could have | Using XML, JSON or a third option, one could easily add new nodes to the REPS System | It could speed up testing, but it is not a requirement for the system to function | This would require FR3 |
| FR3 | Being able to quickly spun up REPS using a specific file type | Could have | Could allow for implementing the system quicker and allow for sharing REPS' | It could speed up testing, but it is not a requirement for the system to function | |
| FR4 | Allow for ML Models in the 'Event Nodes' | Should have | This could really expand the use cases for REPS as companies may have their own or develop one for their problem | It is very usefull for this project, but not required to be seen as succesfull | |
| FR5 | Able to detect events | Must have | Detect any events occuring in the system | This is the whole point of the system | |
| FR6 | Graphical user-interface | Wont have | A user interface can make it easy for the developer to setup REPS | This would require too much time without giving anything usefull to determine the viability of the system | |
| FR7 | Automatic RESP creation | Wont have | It could in theory make it so a system could automaticly create a REPS based on system documentation | This is a research topic in itself and is too add this to the project | |

## Non-Functional Requirements

| Requirement ID | Requirement | Prioritization Level | Description | Reason | Comments |
| :------------: | :---------: | :------------------: | :---------: | :----: | :------: |
| NF0 | Should not have a high complexity | Should have | high complexity is in terms of the developer being able to understand the system | The advantage with this is the developer should be able to understand it compared to most MLs | |
| NF1 | Restricted Memory usage | Could have | Risk of high resource usage | I fear that this system might suffer from memory in smaller computers | |
| NF2 | Easy to communicate with | Should have | Should be easy to gain the values which is needed from the REPS | It would make it hard on the people using REPS if this requirement is not fullfilled, deminishing the usefullnes of REPS | |

## Technology Choices

Based on these requirements, I deem that I'm going to need the following technologies, frameworks or patterns:
- Publish Subscribe pattern, MQTT (FR1, FR2, FR3, NF2):
    - A publish subscribe pattern fits best when it comes to communication, the modifiability and adaptability that comes with this pattern fits very well with the communication requirement (FR1) while also allowing for easy adaptability (FR2 & FR3). I want a light weight solution, I there don't see a need for kafka.
- JSON interpreter for saving (FR2, FR3):
    - A relational database dosen't fit with the way REPS is built. The adaptability with JSON or XML will work great with REPS. There neither a need for the performance from a relational database. I will be using JSON as I have implented similar feature in JSON several times, while I've done i far less in XML. I will neither be using AVRO due to the increased time cost.
- C#:
    - I have most experience with this while it not being an slower language to develop in, like C/C++. I also believe that it being object oriented language fits well with REPS. Only drawback I see is that not alot of ML are developed for C# compared to python. This will limit the time of implementing FR4, but will allow me to quicker implement all other features.

Any further specification, like what MQTT implementation to make use of, will be determined later once all other things in this analysis has been determined.

## Diagrams

TODO: INSERT THE PUML DIAGRAMS

## Finalise Technology
Based on the diagrams from previous, I will be using RabbitMQ for MQTT due to it support everything I need while being able to be implemented in C#. It supports all classic MQTT features, but also it supoort Queue priority which I believe will be neccesary in a crisis situation. There should however be a limit to the amount of priority levels, as the higher the max is set, the more CPU resources is required. Also some messages which should expire might be stuck as rabbitMQ expires from the head of the queue. Now I don't believe this will be a problem as long as the end user dosen't send too many messages.

For C# I will have to use the Object type for the output, I could be using dynamic, but as it is casted in runtime to the type that I would might need. I cannot define it as a var because that would still be defined during compile time, and it has to be defined during runtime. Now using object I will have to define a set og legal casting types, so I will lose flexability but I severly decrease the risk of runtime errors with this variable.

A worry that I have, which I do see as a minor problem atm, but the system will only hold one output type to limit memory usage. I will still allow for casting to other types, but is only really intended for that one specific type. And I don't see a reason for disallowing someone to cast to another type, as long as the user knows that cast errors can occur.

I will be using C#'s Roslyn to intrepet a function during runtime. This will allow for full customisation in terms of math and if statements.

The await function releases current thread and allows for the REPS model to operate as a tree when it comes to their dependencies, as a Eventnode requires that the former events has processed before processing itself.

I will have to figure out how to ensure that it actually can become a graph and not a tree. Bit like circular definations.