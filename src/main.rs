use std::{env::args, fs::{self, File}, io::{BufReader, BufRead}};
mod tokenizer;
mod tokens;
use tokenizer::tokenize_file;

fn main() {
    args().for_each(|f| {
        println!("{}", f);
    });

    if args().len() < 2
    {
        println!("No input file");
        return;
    }

    let input_file_path = args().nth(1).unwrap();

    let input_file: Vec<String> = BufReader::new(File::open(input_file_path).expect("no such file")) 
        .lines()
        .map(|l| l.expect("Could not parse line"))
        .collect();


    // input_file.iter().for_each(|f| {println!("{}", f)});

    tokenize_file(input_file);
}
