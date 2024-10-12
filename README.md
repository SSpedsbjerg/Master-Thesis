# **Root-Causing and Event Identification Through Sensor Data**

The readme contains work which is not in the final document but is used to show how and what methodologies used in this project. It will also contain documentation on how to use the program which will be developed along side the project.

This will be updated as the project progress'

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