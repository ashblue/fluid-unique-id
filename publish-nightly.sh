#!/usr/bin/env bash

setup_git() {
    git config --global user.email "travis@travis-ci.org"
    git config --global user.name "Travis CI - Bot"
}

update_nightly_branch() {
    printf "Run nightly update \n"
    git remote add origin-nightly https://${GH_TOKEN}@github.com/ashblue/fluid-unique-id.git
    git subtree split --prefix Assets/com.fluid.unique-id -b nightly
    git push -f origin-nightly nightly:nightly
}

setup_git
update_nightly_branch
